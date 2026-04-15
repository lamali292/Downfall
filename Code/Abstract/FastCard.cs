using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Abstract;

public abstract class FastCard(int cost, CardType type, CardRarity rarity, TargetType target) 
    : ConstructedCardModel(cost, type, rarity, target)
{
    private readonly List<Func<PlayerChoiceContext, CardPlay, Task>> _actionQueue = [];
    private readonly List<(string Verb, string Content)> _targetActions = [];
    private readonly List<string> _otherFragments = [];
    
    public FastCard Attack(int val, int up = 0) {
        WithDamage(val, up);
        _targetActions.Add(("Deal", "{Damage:diff()} damage"));
        _actionQueue.Add(async (ctx, cp) => await CommonActions.CardAttack(this, cp).Execute(ctx));
        return this;
    }

    public FastCard Block(int val, int up = 0) {
        WithBlock(val, up);
        _otherFragments.Add("Gain {Block:diff()} [gold]Block[/gold].");
        _actionQueue.Add(async (ctx, cp) => await CommonActions.CardBlock(this, cp));
        return this;
    }

    public FastCard Power<T>(int val, int up = 0) where T : PowerModel {
        WithPower<T>(val, up);
        var name = ModelDb.Power<T>().Title.GetFormattedText();
        _targetActions.Add(("Apply", $"{{{typeof(T).Name}:diff()}} {name}"));
        _actionQueue.Add(async (ctx, cp) => await MyCommonActions.Apply<T>(this, cp));
        return this;
    }

    public FastCard PowerSelf<T>(int val, int up = 0) where T : PowerModel {
        WithPower<T>(val, up);
        var name = ModelDb.Power<T>().Title.GetFormattedText();
        _otherFragments.Add($"Gain {{{typeof(T).Name}:diff()}} {name}.");
        _actionQueue.Add(async (_, _) => await CommonActions.ApplySelf<T>(this));
        return this;
    }

    public FastCard Effect(Func<PlayerChoiceContext, CardPlay, Task> action, string? description = null) {
        if (description != null) _otherFragments.Add(description.TrimEnd('.') + ".");
        _actionQueue.Add(action);
        return this;
    }

    public FastCard Desc(string text) {
        _otherFragments.Add(text.TrimEnd('.') + ".");
        return this;
    }

    public override List<(string, string)> Localization => [
        ("title", GetType().Name), 
        ("description", BuildDescription())
    ];

    private string BuildDescription() {
        if (_targetActions.Count == 0) return string.Join(" ", _otherFragments);

        if (TargetType == TargetType.AnyEnemy) {
            var list = new List<string>(_otherFragments);
            foreach (var (verb, content) in _targetActions)
                list.Add($"{verb} {content}.");
            return string.Join("\n", list);
        }
        
        var grouped = new List<string>();
        for (var i = 0; i < _targetActions.Count; i++) {
            var current = _targetActions[i];
            if (i > 0 && _targetActions[i - 1].Verb == current.Verb)
                grouped.Add(current.Content);
            else
                grouped.Add($"{current.Verb.ToLower()} {current.Content}");
        }

        var combined = grouped.Count > 1 
            ? string.Join(", ", grouped.SkipLast(1)) + " and " + grouped.Last()
            : grouped[0];
            
        var targetSentence = char.ToUpper(combined[0]) + combined[1..] + GetTargetSuffix() + ".";
            
        var finalParts = new List<string>(_otherFragments) { targetSentence };
        return string.Join(" ", finalParts);
    }

    private string GetTargetSuffix() => TargetType switch {
        TargetType.AllEnemies => " to ALL enemies",
        TargetType.RandomEnemy => " to a random enemy",
        _ => ""
    };

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cp) {
        foreach (var action in _actionQueue) await action(ctx, cp);
    }
}
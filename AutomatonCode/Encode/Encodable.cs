using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

/// <summary>
///     A reusable piece of what an Encode card does once it is part of a Function. Most are a
///     <see cref="ValueEncode" /> (a number that is summed into the Function and played); the rest only edit the
///     Function while it is assembled (Retain, a fixed cost, a bonus at one position) and have no value.
/// </summary>
public abstract class Encodable
{
    /// <summary>Every encode effect.</summary>
    public static readonly IReadOnlyList<Encodable> All = new Encodable[]
    {
        new PowerEncode(),
        new BlockEncode(),
        new DamageEncode(),
        new StrengthEncode(),
        new WeakEncode(),
        new VulnerableEncode(),
        new PoisonEncode(),
        new SoulburnEncode(),
        new EnergyEncode(),
        new DazedEncode(),
        new RetainEncode(),
        new FunctionCostEncode(),
        new StartBlockEncode(),
        new MiddleDamageEncode(),
        new ReplayAtEndEncode()
    };

    /// <summary>The effects that carry a value, in play order (<see cref="ValueEncode.Order" />).</summary>
    public static readonly IReadOnlyList<ValueEncode> Valued =
        All.OfType<ValueEncode>().OrderBy(e => e.Order).ToList();

    /// <summary>Loc key part in <c>encode.json</c>: <c>&lt;MOD PREFIX&gt;&lt;Id&gt;.encode</c> / <c>.compile</c>. Explicit so renaming the class cannot break loc.</summary>
    public abstract string Id { get; }

    /// <summary>Runs when a card with this effect is played (its normal effect) or the Function is played. Nothing by default.</summary>
    public virtual Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        return Task.CompletedTask;
    }

    public virtual IEnumerable<IHoverTip> HoverTips(AbstractModel card)
    {
        return [];
    }

    /// <summary>The text this effect adds to a card, Function or power. Null when it has none.</summary>
    public virtual LocString? GetDescription(AbstractModel card)
    {
        return null;
    }

    /// <summary>
    ///     Called for every source card while the Function is assembled, with the card's slot in the sequence.
    ///     Value effects sum their value into the Function; the others edit it directly.
    /// </summary>
    public virtual void ApplyEncode(FunctionCard function, CardModel sourceCard, FunctionPosition position)
    {
    }

    /// <summary>
    ///     What this effect changes about the Function itself, shown in the Function's Compile list
    ///     (<c>&lt;Id&gt;.compile</c> in <c>encode.json</c>, formatted with the source card's vars). Null when the
    ///     effect has no such entry.
    /// </summary>
    public LocString? GetFunctionNote(CardModel sourceCard)
    {
        var note = new LocString("encode", GetType().GetPrefix() + Id + ".compile");
        if (!note.Exists()) return null;
        sourceCard.DynamicVars.AddTo(note);
        return note;
    }
}

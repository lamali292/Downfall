using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class BurnOut : AutomatonCardModel
{
    public BurnOut() : base(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
        WithDamage(6, 3);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(DownfallTip.Status);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(Owner.Creature.CombatState);
        var statuses = Owner.GetAllCards()
            .Where(c => c.Type is CardType.Status)
            .ToList();

        if (statuses.Count == 0) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingRandomOpponents(Owner.Creature.CombatState)
            .WithHitCount(statuses.Count)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        foreach (var status in statuses) await CardCmd.Exhaust(ctx, status);
    }
}
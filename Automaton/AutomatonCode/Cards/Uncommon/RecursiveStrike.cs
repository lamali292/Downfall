using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class RecursiveStrike : AutomatonCardModel
{
    public RecursiveStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithTip(AutomatonTip.Encode);
        WithTip(typeof(StrikeAutomaton));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        var strike1 = combatState.CreateCard<StrikeAutomaton>(Owner);
        var strike2 = combatState.CreateCard<StrikeAutomaton>(Owner);
        await AutomatonCmd.EncodeCard(strike1, ctx, cardPlay);
        await AutomatonCmd.EncodeCard(strike2, ctx, cardPlay);
    }
}
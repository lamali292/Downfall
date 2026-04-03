using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Basic;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class RecursiveStrike : AutomatonCardModel
{
    public RecursiveStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithTip(DownfallKeyword.Encode);
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

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
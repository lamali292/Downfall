using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Flail : AutomatonCardModel
{
    public Flail() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(7);
        WithPower<ArtifactPower>(1);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(DownfallKeyword.Status);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Card.CombatState);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(cardPlay.Card.CombatState)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await PowerCmd.Apply<ArtifactPower>(Owner.Creature, DynamicVars.Power<ArtifactPower>().BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars.Power<ArtifactPower>().UpgradeValueBy(1);
    }
}
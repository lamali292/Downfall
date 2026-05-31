using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class GrowthPunch : SlimeBossCardModel, IHasConsumeEffect
{
    public GrowthPunch() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4, 1);
        WithBlock(4, 1);
        WithVar("Increase", 4, 1);
        WithTip(SlimeBossTip.Consume);
    }

    public Task ConsumeEffect(PlayerChoiceContext ctx, Creature creature, AttackCommand command, int amount)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].BaseValue);
        DynamicVars.Block.UpgradeValueBy(DynamicVars["Increase"].BaseValue);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
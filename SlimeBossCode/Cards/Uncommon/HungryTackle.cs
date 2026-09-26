using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class HungryTackle : SlimeBossCardModel, IHasConsumeEffect
{
    public HungryTackle() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 1);
        WithEnergy(2, 1);
        WithTags(SlimeBossTag.Tackle);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(SlimeBossTip.Consume);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SlimeBossCmd.Consume(ctx, this, cardPlay);
    }

    public async Task ConsumeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, Creature target)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}

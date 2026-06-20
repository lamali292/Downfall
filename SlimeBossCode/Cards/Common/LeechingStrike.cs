using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class LeechingStrike : SlimeBossCardModel, IHasConsumeEffect
{
    public LeechingStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        WithTags(CardTag.Strike);
        WithTip(SlimeBossTip.Consume);
    }

    public async Task ConsumeEffect(PlayerChoiceContext ctx, Creature creature, AttackCommand command, int amount)
    {
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Move | ValueProp.Unpowered, null);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
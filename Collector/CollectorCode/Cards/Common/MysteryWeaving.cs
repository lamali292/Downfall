using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class MysteryWeaving : CollectorCardModel
{
    public MysteryWeaving() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCalculatedBlock(9, 2, CalcBlock, ValueProp.Move, 3);
    }

    private static decimal CalcBlock(CardModel card, Creature? creature)
    {
        return -PileType.Hand.GetPile(card.Owner).Cards.Count(e => e != card);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = DynamicVars.CalculatedBlock.Calculate(Owner.Creature);
        await CreatureCmd.GainBlock(Owner.Creature, amount, DynamicVars.CalculatedBlock.Props, cardPlay);
    }
}
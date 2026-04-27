using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Endure : ChampCardModel
{
    public Endure() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCalculatedBlock(7, 1, BlockCalc, ValueProp.Move, 3);
        WithTip(typeof(StrengthPower));
        WithIcon<StrengthPower>();
    }

    private static decimal BlockCalc(CardModel card, Creature? creature)
    {
        return card.Owner.Creature.GetPowerAmount<StrengthPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.EnterDefensiveStance(ctx, Owner);
        var block = DynamicVars.CalculatedBlock.Calculate(cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }
}
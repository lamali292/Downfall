using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class DragonsTrove : CollectorCardModel
{
    public DragonsTrove() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPyre();
        WithCards(2);
        WithVar("Reserve", 1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.DrawCollected(ctx, Owner, DynamicVars.Cards.IntValue);
        CollectorEnergy.Gain(Owner, DynamicVars["Reserve"].IntValue);
    }
}
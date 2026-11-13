using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class FootballHelmet : SlimeBossRelicModel
{
    public FootballHelmet() : base(RelicRarity.Uncommon)
    {
        WithEnergy(2);
        WithBlock(4);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return cardPlay.Card.Owner == Owner && SlimeBossCmd.IsTwoEnergyAttack(cardPlay.Card, cardPlay)
            ? MyCommonActions.Block(this)
            : Task.CompletedTask;
    }
}

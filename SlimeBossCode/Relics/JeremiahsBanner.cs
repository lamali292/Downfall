using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class JeremiahsBanner : SlimeBossRelicModel
{
    public JeremiahsBanner() : base(RelicRarity.Uncommon)
    {
        WithTip(SlimeBossTip.Command);
        WithSlimeTip<BruiserSlime>();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner.Creature || cardPlay.Card.Type != CardType.Status) return;
        await SlimeBossCmd.Command<BruiserSlime>(ctx, Owner, 1);
    }
}
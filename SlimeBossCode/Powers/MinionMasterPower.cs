using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

public class MinionMasterPower : SlimeBossPowerModel
{
    public MinionMasterPower()
    {
        WithSlimeTip<BruiserSlime>();
        WithTip(SlimeBossTip.Command);
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner ||
            !(card.Tags.Contains(CardTag.Strike) || card.Tags.Contains(CardTag.Defend)))
            return Task.CompletedTask;
        return SlimeBossCmd.Command<BruiserSlime>(ctx, card.Owner, Amount);
    }
}

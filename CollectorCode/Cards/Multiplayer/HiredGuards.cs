using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Multiplayer;

[Pool(typeof(CollectorCardPool))]
public class HiredGuards : CollectorCardModel
{
    public HiredGuards() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithKindle(10, 4);
        WithTip<Ember>();
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Target?.Player;
        if (player == null) return;
        await CollectorCmd.Kindle(ctx, player, this);
        await DownfallCardCmd.GiveCard<Ember>(player, PileType.Hand, creator: Owner);
    }
}
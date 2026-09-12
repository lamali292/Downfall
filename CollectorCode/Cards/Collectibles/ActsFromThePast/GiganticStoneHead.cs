using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class GiganticStoneHead : ActsFromThePastCard
{
    public GiganticStoneHead() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies, "GIANT_HEAD_ELITE")
    {
        WithUpgradingCardTip<Headcrush>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<Headcrush>(Owner, PileType.Discard, upgraded: IsUpgraded);
    }
}
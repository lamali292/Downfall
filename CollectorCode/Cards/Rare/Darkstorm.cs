using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Darkstorm : CollectorCardModel
{
    public Darkstorm() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithUpgradingCardTip<Blightning>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<Blightning>(Owner, PileType.Hand, upgraded:IsUpgraded);
        await DownfallCardCmd.GiveCard<Blightning>(Owner, PileType.Draw, CardPilePosition.Random, upgraded:IsUpgraded);
        await DownfallCardCmd.GiveCard<Blightning>(Owner, PileType.Discard, upgraded:IsUpgraded);
    }
}
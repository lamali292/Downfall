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
        WithCards(2, 2);
        WithKeyword(CardKeyword.Exhaust);
        this.WithTip<Blightning>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<Blightning>(Owner, PileType.Hand);
        await DownfallCardCmd.GiveCards<Blightning>(Owner, PileType.Draw, DynamicVars.Cards.IntValue,
            CardPilePosition.Random);
    }
}
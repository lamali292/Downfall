using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class DesperatePrayer : AwakenedCardModel
{
    public DesperatePrayer() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithTip<Ceremony>();
        WithKeywords(CardKeyword.Exhaust);
        WithCards(3, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<Ceremony>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
    }
}
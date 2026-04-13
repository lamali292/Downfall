using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class DesperatePrayer : AwakenedCardModel
{
    public DesperatePrayer() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(typeof(Ceremony));
        WithKeywords(CardKeyword.Exhaust);
        WithCards(3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<Ceremony>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
    }
}
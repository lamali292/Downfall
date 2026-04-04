using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class ArcaneNesting : AwakenedCardModel
{
    public ArcaneNesting() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeywords(CardKeyword.Unplayable);
        WithBlock(4, 2);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Pile == null
            || cardPlay.Card.Owner != Owner
            || Pile.Type != PileType.Hand
            || cardPlay.Card.Type != CardType.Power) return;


        await CommonActions.CardBlock(this, cardPlay);
    }
}
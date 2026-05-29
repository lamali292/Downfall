using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class ArcaneNesting : AwakenedCardModel
{
    public ArcaneNesting() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Unplayable);
        WithBlock(4, 2);
    }
    
    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Pile == null
            || cardPlay.Card.Owner != Owner
            || Pile.Type != PileType.Hand
            || cardPlay.Card.Type != CardType.Power) return;


        await CommonActions.CardBlock(this, cardPlay);
    }
}
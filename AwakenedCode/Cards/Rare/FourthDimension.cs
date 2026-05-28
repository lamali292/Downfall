using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class FourthDimension : AwakenedCardModel
{
    public FourthDimension() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = (await CardSelectCmd.FromHand(ctx, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, this)).FirstOrDefault();
        if (card == null) return;
        var clone1 = card.CreateClone();
        var clone2 = card.CreateClone();
        var clone3 = card.CreateClone();
        var a = await CardPileCmd.AddGeneratedCardsToCombat([clone1, clone2, clone3], PileType.Draw, Owner,
            CardPilePosition.Random);
        await CardCmd.Exhaust(ctx, card);
        CardCmd.PreviewCardPileAdd(a, 0.1f, CardPreviewStyle.MessyLayout);
    }
}
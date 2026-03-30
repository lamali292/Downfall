using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class FourthDimension : AwakenedCardModel
{
    public FourthDimension() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = await CommonActions.SelectSingleCard(this, SelectionScreenPrompt, ctx, PileType.Hand);
        if (card == null) return;
        var clone1 = card.CreateClone();
        var clone2 = card.CreateClone();
        var clone3 = card.CreateClone();
        var a = await CardPileCmd.AddGeneratedCardsToCombat([clone1, clone2, clone3], PileType.Draw, true, CardPilePosition.Random);
        await CardCmd.Exhaust(ctx, card);
        CardCmd.PreviewCardPileAdd(a, 0.1f, CardPreviewStyle.MessyLayout);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
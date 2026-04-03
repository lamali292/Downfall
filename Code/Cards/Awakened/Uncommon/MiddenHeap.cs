using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class MiddenHeap : AwakenedCardModel
{
    public MiddenHeap() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(3);
        WithCards(1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var cardsToSelect = PileType.Discard.GetPile(Owner).Cards
            .Concat(PileType.Hand.GetPile(Owner).Cards).Where(c => c.Type is CardType.Status or CardType.Curse)
            .ToList();
        var selected = await CardSelectCmd.FromSimpleGrid(ctx, cardsToSelect, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 2));
        foreach (var cardModel in selected) await CardPileCmd.Add(cardModel, PileType.Hand);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
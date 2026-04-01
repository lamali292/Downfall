using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class TheEncyclopedia : AwakenedCardModel
{
    public TheEncyclopedia() : base(2, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCards(4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var allCards = ModelDb.CardPool<AwakenedCardPool>().AllCards.Concat(ModelDb.CardPool<ColorlessCardPool>().AllCards);

        var cards = CardFactory.GetDistinctForCombat(Owner, allCards, DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration)
            .Select(e=> new CardCreationResult(e)).ToList();
;
        var card = (await CardSelectCmd.FromSimpleGridForRewards(ctx, cards, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 2,2))).ToList();
        
        foreach (var cardModel in card)
        {
            cardModel.EnergyCost.UpgradeBy(-2);
        }
        await CardPileCmd.AddGeneratedCardsToCombat(card, PileType.Hand, true);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}
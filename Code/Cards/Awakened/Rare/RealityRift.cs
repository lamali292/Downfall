using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Rare;


[Pool(typeof(AwakenedCardPool))]
public class RealityRift : AwakenedCardModel
{
    public RealityRift() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<Void>(Owner, PileType.Draw, CardPilePosition.Top);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        // TODO
        var card2 = (await CardSelectCmd.FromSimpleGrid(ctx, [], Owner, prefs)).FirstOrDefault();
        if (card2 == null) return;
        var card = CombatState!.CreateCard(card2, Owner);
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
        
    }

    
}
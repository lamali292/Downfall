using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Soulforge : CollectorCardModel
{
    public Soulforge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await DownfallCardCmd.DrawFromCustomPile(Owner, CollectorPile.Collected);
        if (!result.success) return;
        List<CardModel> cards = [];
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var copy = result.cardAdded.CreateClone();
            copy.UpgradeInternal();
            cards.Add(copy);
           
        } 
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, true);
    }
}
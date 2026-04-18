using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Forgery : CollectorCardModel
{
    public Forgery() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithCards(2, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (Owner.Creature.CombatState == null) return;
        var rng = Owner.RunState.Rng.CombatCardSelection;
        var cards = CollectorPile.Collected.GetPile(Owner).Cards;
        
        if (cards.Count == 0) return;
        CardModel? chosenCard;
        if (cards.Count == 1)
        {
            chosenCard = cards[0];
        }
        else
        {
            chosenCard = await CardSelectCmd.FromChooseACardScreen(
                ctx,
                cards.TakeRandom(3, rng).ToList(),
            Owner,
            true
                );
        }
        if (chosenCard == null) return;
        var copy = chosenCard.CreateClone();
        await CardPileCmd.Add(copy, PileType.Hand);
      
    }
}
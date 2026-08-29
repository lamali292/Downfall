using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Downfall.TestCode;

public class TestContext
{
    public CombatState Combat { get; }   // now settable
    public Player Player { get; }

    public TestContext(CombatState combat, Player player)
    {
        Combat = combat;
        Player = player;
    }


    public async Task<CardModel> AddCardToTopOfDraw<T>() where T : CardModel
    {
        var card = Combat.CreateCard(ModelDb.Card<T>(), Player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Player, CardPilePosition.Top);
        return card;
    }

    public async Task<CardModel> AddCardToHand<T>() where T : CardModel
    {
        var card = Combat.CreateCard(ModelDb.Card<T>(), Player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Player);
        return card;
    }

    public async Task PlayCard(CardModel card, Creature? target = null)
    {
        await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), card, target);
    }
    
}
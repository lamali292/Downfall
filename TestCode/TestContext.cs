using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.TestCode;

// 1. Mark your test methods with this
[AttributeUsage(AttributeTargets.Method)]
public class CardTestAttribute(Type? characterType = null, Type? encounterType = null) : Attribute
{
    public Type? CharacterType { get; } = characterType;
    public Type? EncounterType { get; } = encounterType;
}

// 2. A wrapper passed into every test with helper functions
public class TestContext
{
    public CombatState Combat { get; }
    public Player Player { get; }

    public TestContext(CombatState combat, Player player)
    {
        Combat = combat;
        Player = player;
    }

    public async Task<CardModel> AddCardToTopOfDraw<T>() where T: CardModel
    {
        var model = ModelDb.Card<T>();
        var card = Combat.CreateCard(model, Player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Player, CardPilePosition.Top);
        return card;
    }

    
    public async Task<CardModel> AddCardToHand<T>() where T: CardModel
    {
        var model = ModelDb.Card<T>();
        var card = Combat.CreateCard(model, Player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Player);
        return card;
    }

    public async Task PlayCard(CardModel card, Creature? target = null)
    {
        await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), card, target);
    }
}
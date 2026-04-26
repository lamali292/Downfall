using Downfall.Code.Abstract;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Hexaghost;

public class GiftsFromBeyondPower : HexaghostPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var cards = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Keywords.Contains(DownfallKeywords.Afterlife));
        var card = CardFactory.GetDistinctForCombat(player, cards, Amount, player.RunState.Rng.CombatCardGeneration);
        await CardPileCmd.AddGeneratedCardsToCombat(card, PileType.Hand, player);
    }
}
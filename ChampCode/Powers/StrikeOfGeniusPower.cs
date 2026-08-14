using Champ.ChampCode.Core;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class StrikeOfGeniusPower : ChampPowerModel
{
    public StrikeOfGeniusPower()
    {
        WithTip(DownfallKeyword.Echo);
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var pool = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(e => e.Tags.Contains(CardTag.Strike) && e.Type == CardType.Attack);
        var cards = CardFactory.GetDistinctForCombat(player, pool, Amount,
            player.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var c in cards)
        {
            c.SetToFreeThisTurn();
            c.ToEcho();
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner.Player);
    }
}
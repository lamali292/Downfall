using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Powers;

public class StrikeOfGeniusPlusPower : ChampPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var pool = ModelDb.CardPool<ChampCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(e => e.Tags.Contains(CardTag.Strike));
        var cards = CardFactory.GetDistinctForCombat(player, pool, Amount,
            player.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var c in cards)
        {
            CardCmd.Upgrade(c);
            c.EnergyCost.SetUntilPlayed(0);
            c.AddKeyword(CardKeyword.Ethereal);
            c.AddKeyword(CardKeyword.Exhaust);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner.Player);
    }
}
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     All cards owned by this power's owner cost 0. The owner cannot gain Energy.
///     At the start of your turn, add a random Curse to your hand.
/// </summary>
public sealed class EternalSentencePower() : HermitPowerModel(PowerType.Buff, PowerStackType.Single)
{
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner?.Creature != Owner || card.EnergyCost.CostsX)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = 0;
        return true;
    }


    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player == Owner.Player)
            return 0;
        return base.ModifyMaxEnergy(player, amount);
    }

    public override decimal ModifyEnergyGain(Player player, decimal amount)
    {
        if (player == Owner.Player)
            return 0;
        return base.ModifyEnergyGain(player, amount);
    }


    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;

        var cards = CardFactory.GetDistinctForCombat(
            player,
            ModelDb.CardPool<CurseCardPool>()
                .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint),
            1,
            player.RunState.Rng.CombatCardGeneration).ToList();

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner.Player);
        Flash();
    }
}
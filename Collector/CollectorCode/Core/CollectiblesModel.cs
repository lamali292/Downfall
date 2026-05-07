using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Rewards;
using Collector.CollectorCode.Vfx;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Saves;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Core;

public static class CollectiblesModel
{
    public static List<CardModel> GetCollectibles(Player player)
    {
        return DownfallSaveManager.GetPlayerData(player).CollectorDeck.Select(CardModel.FromSerializable).ToList();
    }

    /// <summary>
    ///     The only safe way to add a collectible.
    ///     Handles local state, network sync, and animation.
    /// </summary>
    public static void SyncAdd(Player player, CardModel card, int essenceCost)
    {
        player.SpendEssence(essenceCost);
        AddCollectible(player, card);

        var target = NTopBarCollectorButton.ButtonPosition + NTopBarCollectorButton.ButtonSize * 0.5f;
        _ = TaskHelper.RunSafely(DownfallCardCmd.AnimateCardFromRewardScreen(target, card, player));

        CustomMessageWrapper.Send(new CollectibleRewardMessage
        {
            WasSkipped = false,
            Card = card.ToSerializable(),
            EssenceCost = essenceCost,
            SenderId = LocalContext.NetId!.Value
        });
    }

    internal static void AddCollectible(Player player, CardModel card)
    {
        DownfallSaveManager.GetPlayerData(player).CollectorDeck.Add(card.ToSerializable());
        NTopBarCollectorButton.RefreshButton();
    }

    public static void ClearCollectibles(Player player)
    {
        DownfallSaveManager.GetPlayerData(player).CollectorDeck.Clear();
    }
}
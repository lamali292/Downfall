using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Piles;
using Collector.CollectorCode.Rewards;
using Collector.CollectorCode.Vfx;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Collector.CollectorCode.Core;

public static class CollectiblesModel
{
    
    
    public static SavedSpireField<Player, List<SerializableCard>> CollectorDeck = new(() => [], "CollectorDeck")
    {
        Serializer = (list, writer) =>
        {
            writer.WriteInt(list.Count);
            foreach (var card in list)
                card.Serialize(writer);
        },
        Deserializer = reader =>
        {
            var count = reader.ReadInt();
            var list = new List<SerializableCard>(count);
            for (var i = 0; i < count; i++)
            {
                var card = new SerializableCard();
                card.Deserialize(reader);
                list.Add(card);
            }
            return list;
        }
    };
    
    public static List<CardModel> GetCollectibles(Player player)
    {
        return CollectorDeck.Get(player)?.Select(CardModel.FromSerializable).ToList() ?? [];
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
        _ = TaskHelper.RunSafely(DownfallCardCmd.AnimateCardFromRewardScreen(CollectorPile.Collected, card, player));

        CustomMessageWrapper.Send(new CollectibleRewardMessage
        {
            WasSkipped = false,
            Card = card.ToSerializable(),
            EssenceCost = essenceCost
        });
    }

    internal static void AddCollectible(Player player, CardModel card)
    {
        CollectorDeck.Set(player, [..CollectorDeck.Get(player)??[], card.ToSerializable() ]);
        NTopBarCollectorButton.RefreshButton();
    }

    public static void ClearCollectibles(Player player)
    {
        CollectorDeck.Set(player, []);
    }
}
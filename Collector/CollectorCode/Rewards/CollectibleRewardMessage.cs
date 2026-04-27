using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Utils.CustomReward;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Collector.CollectorCode.Rewards;

public class CollectibleRewardMessage : CustomRewardMessage
{
    public SerializableCard? Card { get; set; }
    public int EssenceCost { get; set; }
    public override LogLevel LogLevel => LogLevel.Debug;

    public override void Serialize(PacketWriter writer)
    {
        writer.WriteBool(wasSkipped);
        writer.WriteInt(EssenceCost);
        Card!.Serialize(writer);
    }

    public override void Deserialize(PacketReader reader)
    {
        wasSkipped = reader.ReadBool();
        EssenceCost = reader.ReadInt();
        var card = new SerializableCard();
        card.Deserialize(reader);
        Card = card;
    }

    public override void Initialize(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.RegisterMessageHandler<CollectibleRewardMessage>(HandleMessage);
    }

    public override void Dispose(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.UnregisterMessageHandler<CollectibleRewardMessage>(HandleMessage);
    }

    private static void HandleMessage(CollectibleRewardMessage message, ulong senderId)
    {
        if (message.wasSkipped || message.Card == null) return;
        var player = RunManager.Instance.State?.GetPlayer(senderId);
        if (player == null) return;

        player.SpendEssence(message.EssenceCost);
        var cardModel = CardModel.FromSerializable(message.Card);
        CollectiblesModel.AddCollectible(player, cardModel);

        // Only animate when observing another player's reward
        // Local player already animated inside SyncAdd
        if (LocalContext.IsMe(player)) return;
        var container = NRun.Instance?.GlobalUi.MultiplayerPlayerContainer;
        var stateNode = container?.GetChildren()
            .OfType<NMultiplayerPlayerState>()
            .FirstOrDefault(s => s.Player == player);
        if (stateNode != null)
            _ = TaskHelper.RunSafely(stateNode.AnimateCardObtained(cardModel));
    }
}
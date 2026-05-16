using BaseLib.Abstracts;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Collector.CollectorCode.Rewards;

public class CollectibleRewardMessage : ICustomMessage
{
    public bool WasSkipped { get; set; }
    public SerializableCard? Card { get; set; }
    public int EssenceCost { get; set; }

    public bool ShouldBroadcast => false;
    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.Debug;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteBool(WasSkipped);
        writer.WriteInt(EssenceCost);
        Card!.Serialize(writer);
    }

    public void Deserialize(PacketReader reader)
    {
        WasSkipped = reader.ReadBool();
        EssenceCost = reader.ReadInt();
        var card = new SerializableCard();
        card.Deserialize(reader);
        Card = card;
    }

    public void HandleMessage(ulong senderId)
    {
        if (WasSkipped || Card == null) return;
        var player = RunManager.Instance.DebugOnlyGetState()?.GetPlayer(senderId);
        if (player == null) return;

        player.SpendEssence(EssenceCost);
        var cardModel = CardModel.FromSerializable(Card);
        CollectiblesModel.AddCollectible(player, cardModel);

        if (LocalContext.IsMe(player)) return;
        var container = NRun.Instance?.GlobalUi.MultiplayerPlayerContainer;
        var stateNode = container?.GetChildren()
            .OfType<NMultiplayerPlayerState>()
            .FirstOrDefault(s => s.Player == player);
        if (stateNode != null)
            _ = TaskHelper.RunSafely(stateNode.AnimateCardObtained(cardModel));
    }
}
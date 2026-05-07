using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guardian.GuardianCode.Rewards;

public class CardsAddedMessage : ICustomMessage
{
    public bool WasSkipped { get; set; }
    public List<SerializableCard> Cards { get; init; } = [];
    public ulong SenderId { get; set; }

    public bool ShouldBroadcast => false;
    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.Debug;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteBool(WasSkipped);
        writer.WriteULong(SenderId);
        writer.WriteInt(Cards.Count);
        foreach (var card in Cards)
            card.Serialize(writer);
    }

    public void Deserialize(PacketReader reader)
    {
        WasSkipped = reader.ReadBool();
        SenderId = reader.ReadULong();
        var count = reader.ReadInt();
        for (var i = 0; i < count; i++)
        {
            var card = new SerializableCard();
            card.Deserialize(reader);
            Cards.Add(card);
        }
    }

    public void HandleMessage()
    {
        if (WasSkipped || Cards.Count == 0) return;
        var player = RunManager.Instance.State?.GetPlayer(SenderId);
        if (player == null) return;
        var cards = Cards.Select(CardModel.FromSerializable);

        CardPileCmd.Add(cards, PileType.Deck);
        if (LocalContext.IsMe(player)) return;
        var container = NRun.Instance?.GlobalUi.MultiplayerPlayerContainer;
        var stateNode = container?.GetChildren()
            .OfType<NMultiplayerPlayerState>()
            .FirstOrDefault(s => s.Player == player);
        if (stateNode == null) return;
        foreach (var cardModel in Cards.Select(CardModel.FromSerializable))
            _ = TaskHelper.RunSafely(stateNode.AnimateCardObtained(cardModel));
    }
}
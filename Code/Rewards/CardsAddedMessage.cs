using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.Code.Rewards;

public class CardsAddedMessage : CustomRewardMessage
{
    public List<SerializableCard> Cards { get; init; } = [];
    public override LogLevel LogLevel => LogLevel.Debug;

    public override void Serialize(PacketWriter writer)
    {
        writer.WriteBool(wasSkipped);
        writer.WriteInt(Cards.Count);
        foreach (var card in Cards)
            card.Serialize(writer);
    }

    public override void Deserialize(PacketReader reader)
    {
        wasSkipped = reader.ReadBool();
        var count = reader.ReadInt();
        for (var i = 0; i < count; i++)
        {
            var card = new SerializableCard();
            card.Deserialize(reader);
            Cards.Add(card);
        }
    }

    public override void Initialize(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.RegisterMessageHandler<CardsAddedMessage>(HandleMessage);
    }

    public override void Dispose(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.UnregisterMessageHandler<CardsAddedMessage>(HandleMessage);
    }

    private static void HandleMessage(CardsAddedMessage message, ulong senderId)
    {
        if (message.wasSkipped || message.Cards.Count == 0) return;
        var player = RunManager.Instance.State?.GetPlayer(senderId);
        if (player == null) return;
        var cards = message.Cards.Select(CardModel.FromSerializable);

        CardPileCmd.Add(cards, PileType.Deck);
        if (LocalContext.IsMe(player)) return;
        var container = NRun.Instance?.GlobalUi.MultiplayerPlayerContainer;
        var stateNode = container?.GetChildren()
            .OfType<NMultiplayerPlayerState>()
            .FirstOrDefault(s => s.Player == player);
        if (stateNode == null) return;
        foreach (var cardModel in message.Cards.Select(CardModel.FromSerializable))
            _ = TaskHelper.RunSafely(stateNode.AnimateCardObtained(cardModel));
    }
}
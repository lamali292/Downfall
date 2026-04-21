using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.Code.Saves;

public class DownfallPlayerData : IPacketSerializable
{
    [JsonPropertyName("net_id")] public ulong NetId { get; set; }

    [JsonPropertyName("collector_deck")] public List<SerializableCard> CollectorDeck { get; private set; } = [];

    [JsonPropertyName("essence")] public int Essence { get; set; }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteULong(NetId);
        writer.WriteInt(Essence);
        writer.WriteList(CollectorDeck);
    }

    public void Deserialize(PacketReader reader)
    {
        NetId = reader.ReadULong();
        Essence = reader.ReadInt();
        CollectorDeck = reader.ReadList<SerializableCard>();
    }
}
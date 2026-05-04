using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.DownfallCode.Saves;

public class DownfallPlayerData : IPacketSerializable
{
    [JsonPropertyName("net_id")] public ulong NetId { get; set; }

    [JsonPropertyName("collector_deck")] public List<SerializableCard> CollectorDeck { get; set; } = [];

    [JsonPropertyName("essence")] public int Essence { get; set; }
    
    [JsonPropertyName("snecko_pools")] public List<ModelId> SneckoPools { get; set; } = [];


    public void Serialize(PacketWriter writer)
    {
        writer.WriteULong(NetId);
        writer.WriteInt(Essence);
        writer.WriteList(CollectorDeck);
        writer.WriteFullModelIdList(SneckoPools);
    }

    public void Deserialize(PacketReader reader)
    {
        NetId = reader.ReadULong();
        Essence = reader.ReadInt();
        CollectorDeck = reader.ReadList<SerializableCard>();
        SneckoPools = reader.ReadFullModelIdList();
    }
}
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.DownfallCode.Saves;

public class DownfallRunData : ISaveSchema, IPacketSerializable
{
    [JsonPropertyName("player_data")] public List<DownfallPlayerData> PlayerData { get; set; } = [];

    public void Serialize(PacketWriter writer)
    {
        writer.WriteInt(SchemaVersion);
        writer.WriteInt(PlayerData.Count);
        foreach (var pData in PlayerData) pData.Serialize(writer);
    }

    public void Deserialize(PacketReader reader)
    {
        SchemaVersion = reader.ReadInt();
        var count = reader.ReadInt();
        PlayerData = [];
        for (var i = 0; i < count; i++)
        {
            var pData = new DownfallPlayerData();
            pData.Deserialize(reader);
            PlayerData.Add(pData);
        }
    }

    [JsonPropertyName("schema_version")] public int SchemaVersion { get; set; } = 1;
}
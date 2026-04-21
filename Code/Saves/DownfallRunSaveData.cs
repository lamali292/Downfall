using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.Code.Saves;

/*
public class Test() : CustomSingletonModel(true, true)
{

    public override Task BeforeCombatStart()
    {

        DownfallSaveManager.MyRunData.TestValue = 0;
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {

    }
}
*/

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
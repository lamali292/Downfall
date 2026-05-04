using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Downfall.DownfallCode.Utils.ModdedSaves;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Saves;

public static class DownfallSaveManager
{
    [ModSave] public static DownfallRunData MyRunData = new();


    public static DownfallPlayerData GetPlayerData(Player player)
    {
        var netId = player.NetId;
        var data = MyRunData.PlayerData.Find(p => p.NetId == netId);
        if (data != null) return data;
        data = new DownfallPlayerData { NetId = netId };
        MyRunData.PlayerData.Add(data);
        return data;
    }
}
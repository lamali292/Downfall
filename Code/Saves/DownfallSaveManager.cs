using Downfall.Code.Utils.ModdedSaves;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Saves;

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
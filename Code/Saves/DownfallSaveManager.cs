using Downfall.Code.Utils.ModdedSaves;

namespace Downfall.Code.Saves;

public static class DownfallSaveManager
{
    [ModSave] public static DownfallRunData MyRunData = new();
    
    
    public static DownfallPlayerData GetPlayerData(ulong netId)
    {
        var data = MyRunData.PlayerData.Find(p => p.NetId == netId);
        if (data != null) return data;
        data = new DownfallPlayerData { NetId = netId };
        MyRunData.PlayerData.Add(data);
        return data;
    }
}

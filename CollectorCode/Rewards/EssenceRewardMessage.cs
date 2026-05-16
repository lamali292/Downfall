using BaseLib.Abstracts;
using Collector.CollectorCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Rewards;

public class EssenceRewardMessage : ICustomMessage
{
    public bool WasSkipped { get; set; }
    public int Amount { get; set; }

    public bool ShouldBroadcast => false;
    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.Debug;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteBool(WasSkipped);
        writer.WriteInt(Amount);
    }

    public void Deserialize(PacketReader reader)
    {
        WasSkipped = reader.ReadBool();
        Amount = reader.ReadInt();
    }

    public void HandleMessage(ulong senderId)
    {
        if (WasSkipped) return;
        var player = RunManager.Instance.DebugOnlyGetState()?.GetPlayer(senderId);
        if (player == null) return;
        player.AddEssence(Amount);
        if (LocalContext.IsMe(player)) return;

        var stateNode = NRun.Instance?.GlobalUi.MultiplayerPlayerContainer
            .GetChildren()
            .OfType<NMultiplayerPlayerState>()
            .FirstOrDefault(s => s.Player == player);
        if (stateNode != null)
            _ = TaskHelper.RunSafely(AnimateEssenceObtained(stateNode));
    }

    private static async Task AnimateEssenceObtained(NMultiplayerPlayerState stateNode)
    {
        await stateNode.WaitUntilNextTweenTime();
        var node = new TextureRect
        {
            Texture = ResourceLoader.Load<Texture2D>("res://Collector/images/ui/esse.png"),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            CustomMinimumSize = new Vector2(32, 32)
        };
        await stateNode.ObtainedAnimation(node);
        node.QueueFreeSafely();
    }
}
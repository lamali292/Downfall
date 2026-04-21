using BaseLib.Abstracts;
using Downfall.Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Rewards;

public class EssenceRewardMessage : CustomRewardMessage
{
    public int Amount { get; set; }
    public override LogLevel LogLevel => LogLevel.Debug;

    public override void Serialize(PacketWriter writer)
    {
        writer.WriteBool(wasSkipped);
        writer.WriteInt(Amount);
    }

    public override void Deserialize(PacketReader reader)
    {
        wasSkipped = reader.ReadBool();
        Amount = reader.ReadInt();
    }

    public override void Initialize(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.RegisterMessageHandler<EssenceRewardMessage>(HandleMessage);
    }

    public override void Dispose(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.UnregisterMessageHandler<EssenceRewardMessage>(HandleMessage);
    }

    private static void HandleMessage(EssenceRewardMessage message, ulong senderId)
    {
        if (message.wasSkipped) return;
        var player = RunManager.Instance.State?.GetPlayer(senderId);
        if (player == null) return;
        player.AddEssence(message.Amount);
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

        var node = new TextureRect();
        node.Texture = ResourceLoader.Load<Texture2D>("res://Downfall/images/ui/collector/esse.png");
        node.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        node.CustomMinimumSize = new Vector2(32, 32);

        await stateNode.ObtainedAnimation(node);
        node.QueueFreeSafely();
    }
}
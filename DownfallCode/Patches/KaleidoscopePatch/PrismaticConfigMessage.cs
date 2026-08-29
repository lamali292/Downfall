using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

public struct PrismaticConfigMessage : INetMessage
{
    public ulong OwnerNetId;
    public PrismaticMode PrismaticMode;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteULong(OwnerNetId);
        writer.WriteEnum(PrismaticMode);
    }

    public void Deserialize(PacketReader reader)
    {
        OwnerNetId = reader.ReadULong();
        PrismaticMode = reader.ReadEnum<PrismaticMode>();
    }

    public bool ShouldBroadcast => true;
    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.Debug;
    public bool ShouldBuffer => false;
}
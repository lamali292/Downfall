using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace Downfall.Code.Actions;

public struct NetDrawCardsAction : INetAction
{
    public uint Count;

    public GameAction ToGameAction(Player player)
    {
        return new DrawCardsAction(player, Count, true);
    }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteUInt(Count, 4);
    }

    public void Deserialize(PacketReader reader)
    {
        Count = reader.ReadUInt(4);
    }
}
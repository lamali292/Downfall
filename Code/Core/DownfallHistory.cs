using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core;

public class DownfallHistory : AbstractModel
{
    private static readonly SpireField<Player, DownfallHistory> Trackers =
        new(player =>
        {
            var mutable = ModelDb.GetById<DownfallHistory>(ModelDb.GetId<DownfallHistory>()).ToMutable(player);
            return mutable;
        });
    
    private DownfallHistory ToMutable(Player player)
    {
        var mutable = (DownfallHistory)MutableClone();
        mutable._player = player;
        return mutable;
    }
    
    public static DownfallHistory Get(Player player) => Trackers[player]!;
    

    public override bool ShouldReceiveCombatHooks => true;
    private Player? _player;
    public int UnusedBlockLastTurn { get; private set; }

    private Player Owner => _player ?? throw new InvalidOperationException("Not mutable");

    
    public override Task BeforeTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side == CombatSide.Player || _player == null) return Task.CompletedTask;
        UnusedBlockLastTurn = Owner.Creature.Block;
        return Task.CompletedTask;
    }
}
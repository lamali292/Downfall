using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.Core;

public sealed class PlayerField<TVal>
{
    private readonly SpireField<PlayerCombatState, TVal> _inner;

    // player.PlayerCombatState is only replaced by a fresh instance when the NEXT combat starts, not
    // when the current one ends - it stays set (and its data readable) through the whole map/shop/rest
    // site window in between. Clearing here, rather than requiring every consuming model to remember a
    // Clear() call in AfterCombatEnd, closes that window immediately instead of leaving stale data
    // visible until the next combat happens to overwrite it.
    public PlayerField(Func<TVal?> defaultVal)
    {
        _inner = new SpireField<PlayerCombatState, TVal>(defaultVal);
        CombatManager.Instance.CombatEnded += _ => Clear();
    }

    public PlayerField(Func<PlayerCombatState, TVal?> defaultVal)
    {
        _inner = new SpireField<PlayerCombatState, TVal>(defaultVal);
        CombatManager.Instance.CombatEnded += _ => Clear();
    }

    public void Clear()
    {
         _inner._table.Clear();
    }

    public TVal? this[Player? obj]
    {
        get => Get(obj);
        set => Set(obj, value);
    }

    public TVal? this[PlayerCombatState? obj]
    {
        get => Get(obj);
        set => Set(obj, value);
    }

    public TVal? Get(Player? obj)
    {
        return Get(obj?.PlayerCombatState);
    }

    public void Set(Player? obj, TVal? val)
    {
        Set(obj?.PlayerCombatState, val);
    }

    public TVal? Get(PlayerCombatState? obj)
    {
        return obj is null ? default : _inner.Get(obj);
    }

    public void Set(PlayerCombatState? obj, TVal? val)
    {
        if (obj is not null) _inner.Set(obj, val);
    }
}
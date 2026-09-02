using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.Core;

public sealed class PlayerField<TVal>
{
    private readonly SpireField<PlayerCombatState, TVal> _inner;

    public PlayerField(Func<TVal?> defaultVal)
    {
        _inner = new SpireField<PlayerCombatState, TVal>(defaultVal);
    }

    public PlayerField(Func<PlayerCombatState, TVal?> defaultVal)
    {
        _inner = new SpireField<PlayerCombatState, TVal>(defaultVal);
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
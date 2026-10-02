using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

/// <summary>
/// The Collector's Reserve: a per-player pool that covers the Energy deficit of any card and is the only
/// currency for cards implementing <c>IUsesCollectorEnergyOnly</c>. Holds the state only; how it is paid
/// and checked lives in <see cref="ReservePaymentRules"/> and the patches in <c>Patches/Reserve*</c>.
/// </summary>
public class CollectorEnergy : CustomSingletonModel
{
    /// <summary>The singleton, set when the game constructs the model. First instance wins.</summary>
    public static CollectorEnergy? Instance { get; private set; }

    private readonly PlayerField<int> _current = new(() => 0);
    private readonly SpireField<CardModel, int> _lastSpent = new(() => 0);

    public CollectorEnergy() : base(HookType.Combat)
    {
        Instance ??= this;
    }

    public event Action<PlayerCombatState, int>? Changed;

    public int Get(Player player) => _current.Get(player);

    public int Get(PlayerCombatState player) => _current.Get(player);

    public void Set(PlayerCombatState player, int amount)
    {
        var clamped = Math.Max(0, amount);
        _current[player] = clamped;
        GD.Print($"[CollectorEnergy] Set fired: player={player.GetHashCode()} value={clamped}");
        Changed?.Invoke(player, clamped);
    }

    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
            if (player.PlayerCombatState != null)
                Set(player.PlayerCombatState, 0);
        return Task.CompletedTask;
    }

    /// <summary>Records how much Reserve the last play of <paramref name="card"/> took.</summary>
    public void RecordSpent(CardModel card, int amount) => _lastSpent[card] = amount;

    public bool WasSpentOn(CardModel card) => _lastSpent[card] > 0;
    public int AmountSpentOn(CardModel card) => _lastSpent[card];
}

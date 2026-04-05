// ChampModel.cs
using System.Runtime.CompilerServices;
using BaseLib.Utils;
using Downfall.Code.Events;
using Downfall.Code.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Champ;

public class ChampModel : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    private static readonly SpireField<Player, ChampStanceModel> ActiveStance = new(DownfallModelDb.ChampStance<NoChampStance>);
    private static readonly ConditionalWeakTable<Player, NChampStanceDisplay> StanceDisplays = new();

    public static T GetStanceAs<T>(Player player) where T : ChampStanceModel
        => (ActiveStance[player] as T)!;

    public static ChampStanceModel GetStanceModel(Player player)
        => ActiveStance[player] ?? DownfallModelDb.ChampStance<NoChampStance>();

    public static bool IsInStance<T>(Player player) where T : ChampStanceModel
        => ActiveStance[player] is T;

    private static NChampStanceDisplay? GetDisplay(Player player)
        => StanceDisplays.TryGetValue(player, out var d) ? d : null;

    private static void RegisterDisplay(Player player, NChampStanceDisplay display)
        => StanceDisplays.AddOrUpdate(player, display);

    public static void RefreshDisplay(Player player)
        => GetDisplay(player)?.Refresh();

    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
        => await SetStance(ctx, player, DownfallModelDb.ChampStance<T>());

    private static async Task SetStance(PlayerChoiceContext ctx, Player player, ChampStanceModel newCanonical)
    {
        var current = ActiveStance[player];
        if (current?.GetType() == newCanonical.GetType()) return;

        if (current != null)
            await current.OnExit(ctx);

        var mutable = newCanonical.ToMutable(player);
        ActiveStance[player] = mutable;
        await mutable.OnEnter(ctx);

        await DownfallHook.OnStanceChange(ctx, player, current!, ActiveStance[player]!);

        Callable.From(() =>
        {
            if (newCanonical is NoChampStance)
            {
                GetDisplay(player)?.Refresh(); // QueueFrees itself
            }
            else
            {
                var existing = GetDisplay(player);
                if (existing == null || !GodotObject.IsInstanceValid(existing))
                {
                    var display = NChampStanceDisplay.Show(player);
                    if (display != null) RegisterDisplay(player, display);
                }
                else
                {
                    existing.Refresh();
                }
            }
        }).CallDeferred();
    }

    public override async Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return;
        foreach (var player in state.Players)
            ActiveStance[player] = DownfallModelDb.ChampStance<NoChampStance>();
    }
}
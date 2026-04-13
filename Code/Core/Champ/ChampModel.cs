using BaseLib.Utils;
using Downfall.Code.Events;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Champ;

public class ChampModel : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    // Replaces the ConditionalWeakTable
    private static readonly SpireField<Player, StanceModel> ActiveStance = new(DownfallModelDb.ChampStance<NoneStance>);

    
    public static T? GetStanceAs<T>(Player player) where T : StanceModel
        => ActiveStance[player] as T;

    public static StanceModel? GetStanceModel(Player player)
        => ActiveStance[player];

    public static bool IsInStance<T>(Player player) where T : StanceModel
        => ActiveStance[player] is T;
    
    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player) where T : StanceModel
        => await SetStance(ctx, player, DownfallModelDb.ChampStance<T>());

    private static async Task SetStance(PlayerChoiceContext ctx, Player player, StanceModel newCanonical)
    {
        var current = ActiveStance[player];
        if (current?.GetType() == newCanonical.GetType()) return;

        if (current != null)
            await current.OnExit(ctx);

        {
            var mutable = newCanonical.ToMutable(player);
            ActiveStance[player] = mutable;
            await mutable.OnEnter(ctx);
        }

        await DownfallHook.OnStanceChange(ctx, player, current!, ActiveStance[player]!);
    }
}

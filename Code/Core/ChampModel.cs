using System.Runtime.CompilerServices;
using Downfall.Code.Character;
using Downfall.Code.Events;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core;


public enum ChampStance
{
    None,
    Berserker,
    Defensive,
    Ultimate,
    Gladiator
}

public class ChampModel : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;
    
    private static readonly ConditionalWeakTable<Player, StrongBox<ChampStance>> Stances = new();

    public static ChampStance GetStance(Player? player)
    {
        return player == null ? ChampStance.None : Stances.GetOrCreateValue(player).Value;
    }

    public static bool IsInStance(Player? player, ChampStance stance)
    {
        return GetStance(player) == stance;
    }

    public static async Task SetStance(PlayerChoiceContext ctx, Player player, ChampStance newStance)
    {
        var oldStance = GetStance(player);
        if (oldStance == newStance) return;
        Stances.GetOrCreateValue(player).Value = newStance;
        await DownfallHook.OnStanceChange(ctx, player, oldStance, newStance);
    }

    public override Task BeforeCombatStart()
    {
        Stances.Clear();
        return Task.CompletedTask;
    }


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        /*var player = cardPlay.Card.Owner;
        if (player.Character is not Champ) return;

        var next = GetStance(player) switch
        {
            ChampStance.None      => ChampStance.Berserker,
            ChampStance.Berserker => ChampStance.Defensive,
            ChampStance.Defensive => ChampStance.Ultimate,
            _                     => ChampStance.None
        };
        DownfallMainFile.Logger.Info(next.ToString());
        var ctx = new HookPlayerChoiceContext(this, LocalContext.NetId.Value, cardPlay.Card.Owner.Creature.CombatState,
            GameActionType.Combat);
        await SetStance(ctx, player, next);*/
    }
}


[HarmonyPatch(typeof(Log), nameof(Log.Error))]
public static class LogErrorPatch
{
    [HarmonyPrefix]
    static bool DowngradeLocErrors(string text)
    {
        if (text.StartsWith("Localization formatting error!"))
        {
            //Log.Warn(text.Replace("Localization formatting error!", "Localization formatting warning:"));
            return false;
        }
        return true;
    }
}
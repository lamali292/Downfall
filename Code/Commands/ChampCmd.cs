using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Commands;

public class ChampCmd
{
    public static async Task EnterBerserkerStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<BerserkerChampStance>(ctx, player);
    }

    public static async Task EnterDefensiveStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<DefensiveChampStance>(ctx, player);
    }

    public static async Task EnterUltimateStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<UltimateChampStance>(ctx, player);
    }

    public static async Task EnterGladiatorStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<GladiatorChampStance>(ctx, player);
    }

    public static async Task EnterStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
    {
        await ChampModel.SetStance<T>(ctx, player);
    }

    public static async Task ClearStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<NoChampStance>(ctx, player);
    }

    public static async Task PlayFinisher(PlayerChoiceContext ctx, CardPlay cardPlay, bool skipClear = false)
    {
        var player = cardPlay.Card.Owner;
        var m = player.ChampStance();
        if (!m.HasFinisher) return;
        await m.Finisher(ctx);
        if (skipClear) return;
        await ClearStance(ctx, player);
    }
}
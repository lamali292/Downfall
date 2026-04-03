using Downfall.Code.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Commands;

public class ChampCmd
{
    public static async Task BerserkerStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance(ctx, player, ChampStance.Berserker);
    }
    
    public static async Task DefensiveStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance(ctx, player, ChampStance.Defensive);
    }
    
    public static async Task NoStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance(ctx, player, ChampStance.None);
    }
    
    public static async Task UltimateStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance(ctx, player, ChampStance.Ultimate);
    }
    
    public static async Task GladiatorStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance(ctx, player, ChampStance.Gladiator);
    }
}
using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Commands;

public class ChampCmd
{
    public static async Task EnterBerserkerStance(PlayerChoiceContext ctx, Player player)
        => await ChampModel.SetStance<BerserkerStance>(ctx, player);

    public static async Task EnterDefensiveStance(PlayerChoiceContext ctx, Player player)
        => await ChampModel.SetStance<DefensiveStance>(ctx, player);

    public static async Task EnterUltimateStance(PlayerChoiceContext ctx, Player player)
        => await ChampModel.SetStance<UltimateStance>(ctx, player);

    public static async Task EnterGladiatorStance(PlayerChoiceContext ctx, Player player)
        => await ChampModel.SetStance<GladiatorStance>(ctx, player);
    
    public static async Task EnterStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
        => await ChampModel.SetStance<T>(ctx, player);

    public static async Task ClearStance(PlayerChoiceContext ctx, Player player)
        => await ChampModel.SetStance<NoneStance>(ctx, player);

    public static async Task PlayFinisher(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        var m = ChampModel.GetStanceModel(player);
        if (!m.HasFinisher) return;
        await m.Finisher(ctx);
        await ClearStance(ctx, player);
        
    }
}
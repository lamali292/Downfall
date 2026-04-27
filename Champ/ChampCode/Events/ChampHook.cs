using Champ.ChampCode.Core;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Events;

public static class ChampHook
{
   

    public static Task OnFinisher(ICombatState cs, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return DownfallHook.Dispatch<IOnFinisher>(cs, ctx, m => m.OnFinisher(ctx, cardPlay));
    }


    public static Task OnChampStanceChange(ICombatState cs, PlayerChoiceContext ctx, Player player,
        ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        return DownfallHook.Dispatch<IOnChampStanceChange>(cs, ctx, m => m.OnChampStanceChange(ctx, player, oldStance, newStance));
    }
    

    public static int ModifySkillBonus<TPower>(ICombatState cs, ChampStanceModel stanceModel, int baseAmount)
        where TPower : PowerModel
    {
        return DownfallHook.Aggregate<IModifySkillBonus, int>(cs, baseAmount,
            (m, current) => m.ModifySkillBonus<TPower>(stanceModel, current));
    }

    public static int ModifyCounterStrikeCount(ICombatState cs, Player player, int baseAmount)
    {
        return DownfallHook.Aggregate<IModifyCounterStrikeCount, int>(cs, baseAmount,
            (m, current) => m.ModifyCounterStrikeCount(player, current));
    }
    
    public static bool IgnoreChargeCap(ICombatState cs, Player player)
    {
        return DownfallHook.Any<IIgnoreChampChargeCap>(cs, m => m.IgnoreChargeCap(player));
    }

    public static int ModifyFinisherBonus(ICombatState cs, ChampStanceModel stanceModel, int baseAmount)
    {
        return DownfallHook.Aggregate<IModifyFinisherBonus, int>(cs, baseAmount,
            (m, current) => m.ModifyFinisherBonus(stanceModel, current));
    }

    

 
}
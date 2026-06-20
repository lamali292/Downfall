using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Potions;

[Pool(typeof(GuardianPotionPool))]
public class LiquidAccelerant : GuardianPotionModel
{
    public LiquidAccelerant() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithTip(GuardianTip.Accelerate);
        WithTip(GuardianTip.Stasis);
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return GuardianCmd.AccelerateUntilExit(ctx, Owner);
    }
}
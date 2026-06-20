using BaseLib.Extensions;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Potions;

[Pool(typeof(GuardianPotionPool))]
public class PolishingOil : GuardianPotionModel
{
    public PolishingOil() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithTip(GuardianTip.Polish);
        WithVars(new PolishVar(5));
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return GuardianCmd.Polish(ctx, this);
    }
}
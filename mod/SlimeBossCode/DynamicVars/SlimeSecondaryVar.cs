using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.DynamicVars;

public class SlimeSecondaryVar(decimal baseValue) : DynamicVar("Slime", baseValue)
{
    /// <summary>
    /// The value shown to the player, given the amount after <c>IModifySlimeEffects</c>. Display only: the slime
    /// itself must still apply the unmodified amount, or modifiers applied by the game would be counted twice.
    /// </summary>
    public virtual decimal ToPreviewValue(decimal modifiedAmount, SlimeModel slime) => modifiedAmount;
}

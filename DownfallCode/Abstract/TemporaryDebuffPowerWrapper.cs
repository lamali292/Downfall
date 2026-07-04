using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

/// <summary>
/// Workaround for a BaseLib bug (github.com/Alchyr/BaseLib-StS2): CustomTemporaryPowerModel.Type
/// is derived from InternallyAppliedPower.Type (e.g. always Buff for StrengthPower/DexterityPower)
/// and ignores InvertInternalPowerAmount. Relics that gate on debuff detection via
/// PowerModel.GetTypeForAmount (e.g. Unsettling Lamp) then misclassify the wrapper itself as a
/// Buff and skip it, so only the internally-applied power gets doubled while this wrapper's own
/// Amount - which the end-of-turn restore reads - never does. Net effect: a permanent stat leak
/// (e.g. MirePit only gives back half the Strength it took under Unsettling Lamp).
/// Remove this class (and point subclasses back at CustomTemporaryPowerModelWrapper) once BaseLib
/// fixes Type to respect InvertInternalPowerAmount.
/// </summary>
public abstract class TemporaryDebuffPowerWrapper<TModel, TPower> : CustomTemporaryPowerModelWrapper<TModel, TPower>
    where TModel : AbstractModel where TPower : PowerModel
{
    protected override bool InvertInternalPowerAmount => true;
    public override PowerType Type => PowerType.Debuff;
}

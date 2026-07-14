using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;

namespace Downfall.DownfallCode.Abstract;

public abstract class CustomAfflictionModel : AfflictionModel, ICustomModel
{
    /// <summary>
    /// Override this or place your affliction's overlay scene at the default path
    /// used by the base game ("cards/overlays/afflictions/...").
    /// Return null to fall through to the default OverlayPath.
    /// </summary>
    public virtual string? CustomOverlayPath => null;

}


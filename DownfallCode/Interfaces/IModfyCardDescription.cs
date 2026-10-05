using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.Interfaces;

// Public: implemented by cards in standalone submods too (e.g. SlimeBoss, its own assembly since
// the standalone-submods effort), not just code compiled into Downfall.dll itself.
public interface IModfyCardDescription
{
    LocString ModifyDescription(LocString oldLocString);
}
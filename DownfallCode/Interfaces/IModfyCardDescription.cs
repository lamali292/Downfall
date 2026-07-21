using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.Interfaces;

internal interface IModfyCardDescription
{
    LocString ModifyDescription(LocString oldLocString);
}
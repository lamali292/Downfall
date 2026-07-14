using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.Interfaces;

interface IModfyCardDescription
{
    LocString ModifyDescription(LocString oldLocString);
}
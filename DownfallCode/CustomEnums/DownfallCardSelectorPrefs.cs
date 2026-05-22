using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.CustomEnums;

public struct DownfallCardSelectorPrefs
{
    public static LocString ToTopSelectionPrompt => new("card_selection", "TO_TOP");
    public static LocString ToHandSelectionPrompt => new("card_selection", "TO_HAND");
    public static LocString ApplySelectionPrompt => new("card_selection", "TO_APPLY");
    public static LocString StasisSelectionPrompt => new("card_selection", "TO_STASIS");
    public static LocString PlaySelectionPrompt => new("card_selection", "TO_PLAY");
    public static LocString ConjureSelectionPrompt => new("card_selection", "TO_CONJURE");
}
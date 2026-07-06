using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.CustomEnums;

public struct DownfallCardSelectorPrefs
{
    public static LocString ToTopSelectionPrompt => new("card_selection", "DOWNFALL-TO_TOP");
    public static LocString ToHandSelectionPrompt => new("card_selection", "DOWNFALL-TO_HAND");
    public static LocString ToDeckSelectionPrompt => new("card_selection", "DOWNFALL-TO_DECK");
    public static LocString ToAllPlayerHandSelectionPrompt => new("card_selection", "DOWNFALL-TO_OTHER_HANDS");
    public static LocString ApplySelectionPrompt => new("card_selection", "DOWNFALL-TO_APPLY");
    public static LocString StasisSelectionPrompt => new("card_selection", "DOWNFALL-TO_STASIS");
    public static LocString PlaySelectionPrompt => new("card_selection", "DOWNFALL-TO_PLAY");
    public static LocString ConjureSelectionPrompt => new("card_selection", "DOWNFALL-TO_CONJURE");
    public static LocString RetainSelectionPrompt => new("card_selection", "DOWNFALL-TO_RETAIN");
    public static LocString AddEtherealSelectionPrompt => new("card_selection", "DOWNFALL-ADD_ETHEREAL");
}
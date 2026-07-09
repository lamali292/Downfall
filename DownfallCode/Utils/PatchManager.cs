using Downfall.DownfallCode.Patches;
using HarmonyLib;

namespace Downfall.DownfallCode.Utils;

public class DownfallPatchManager
{
    
     public static void HarmonyPatches()
    {
        ModPatcher.Create(DownfallMainFile.ModId, DownfallMainFile.Logger)
        .Add(typeof(ModelDbInitPatch))
        
        .Add(typeof(CombatUiActivatePatch))
        .Add(typeof(ModifyDamageInternalPatch))
        .Add(typeof(AfflictionModelOverlayPathPatch))
        .Add(typeof(CardDescriptionPatch))
        .Add(typeof(GetCardTextPatch))
        .Add(typeof(SetCardContextPatch))
        .Add(typeof(PatchCardTitle))
        
        .Add(typeof(ReplayCountPatch))
        
        .Add(typeof(VigorRetainPatch))
        .Add(typeof(NCardPortraitPatch))
        .Add(typeof(KeywordColorPatch))
       
        .Add(typeof(AddArtistHoverTipPatch))
        
        .Add(typeof(RichTextEffectRegistryPatch))
        .Add(typeof(MaxUpgradeLevelPatch))
        
        .Add(typeof(AddExtraHpBarPatch))
        .Add(typeof(CustomIntentLabelPatch))
        
        .Add(typeof(CardOverlayPatch))
        .Add(typeof(CardColorPatch))
        
        .Add(typeof(ColorfulPhilosophersPatch))
        .Add(typeof(FindOnTablePatch))
        .Add(typeof(FromChooseACardScreenPatch))
        .Add(typeof(GetModdedLocTablesPatch))
        .Add(typeof(LocManagerPatch))
        .Add(typeof(ModifyCardDescriptionPatch))
        
        .Add(typeof(PowerShouldRemoveDueToZeroPatch))
        
        .Add(typeof(SfxOverridePatch))
        .Add(typeof(PlayOneShotPatch))
        .Add(typeof(PlayOneShotDictPatch))
        
        .Add(typeof(NCardUpdateTypePlaquePatch))
        
        .Add(typeof(NCreatureAnimationPatch))
        .Add(typeof(NCreatureDeathAnimationPatch))
        
        .Add(typeof(FakeMerchantAnimationPatch))
        .Add(typeof(PluralRulesPatch))
        .Add(typeof(TranscendenceTransformationPatch))
        
        .Add(typeof(CardModifierGlowGoldPatch))
        .Add(typeof(ForceVisitIndexConsolePatch))
        
        .Add(typeof(TopBarInitializePatch))
        .Add(typeof(CombatPilesContainerPatch))
        
        .Add(typeof(GenericSpendResourcesPatch))
        .Add(typeof(GenericHasEnoughResourcesPatch))
        .Add(typeof(GenericResourceUiPatch))
        
        .Add(typeof(OnClearBlockPatch))
        .Add(typeof(NewRunPatch))
        
        .Add(typeof(DeathInterceptPatch))
        .Add(typeof(CustomPowerIconPatch))
        .Add(typeof(CardOverlayPatches))
        .PatchAll();
        
        
    }
    
}
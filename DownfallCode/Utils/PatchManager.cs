using Downfall.DownfallCode.Audio;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Patches;

namespace Downfall.DownfallCode.Utils;

public class DownfallPatchManager
{
    public static void HarmonyPatches()
    {
        var patcher = ModPatcher.Create(DownfallMainFile.ModId, DownfallMainFile.Logger)
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
            .Add(typeof(AncientSeaGlassConsolePatch))
            .Add(typeof(CreatureNavigationLinkPatch))
            .Add(typeof(FindExistingInstanceForStackingPatch))
            .Add(typeof(IgnoreDexterityPatch))
            .Add(typeof(InvokeSilentDisplayAmountChangedPatch))
            .Add(typeof(NCreditsScreenPatch))
            //.Add(typeof(EnchantmentModelCanEnchantCardVeto))
            .Add(typeof(CardCmdTransformTransformHook))
            .Add(typeof(DeferredInitializationFmodFlushPatch))
            .Add(typeof(ScrollBoxesCustomBundlePatch));
        


        patcher.Add(GameVersion.HasNCardUpdatePortrait
            ? typeof(NCardUpdatePortraitPatch)
            : typeof(NCardReloadPortraitPatch));

        if (GameVersion.HasCardLocation)
            patcher.Add(typeof(ModifyCardPlayResultLocationNewPatch))
                .Add(typeof(AfterModifyingLocationNewPatch));
        else
            patcher.Add(typeof(ModifyCardPlayResultLocationOldPatch))
                .Add(typeof(AfterModifyingLocationOldPatch));

        // Todo : only for 0.110.1
        // /*
        // patcher.Add(typeof(VoidFormBonePatch))
        //     .Add(typeof(ReaperFormBonePatch))
        //     .Add(typeof(SerpentFormBonePatch))
        //     .Add(typeof(EchoFormBonePatch));
        //     */
        FormBonePatcher.Apply(patcher.Harmony, DownfallMainFile.Logger);

        patcher.PatchAll();
    }
}
using BaseLib.Abstracts;
using BaseLib.Patches.Saves;
using BaseLib.Utils;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Godot;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Saves.Runs;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Guardian.GuardianCode;

[ModInitializer(nameof(Initialize))]
public partial class GuardianMainFile : Node
{
    public const string ModId = "Guardian";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CustomLocTableManager.Register("gems");
        RegisterGemSave();
        CardDescriptionRegistry.Register<GuardianCardModel>(DescriptionInjectionPoint.BelowMainText,
            new GemDescriptionSource());

        BundledSubmodLocRegistry.Register(ModId);

        TranscendenceHooks.OnTransformed += CopyGemsToTranscendence;
        CombatUiHooks.Register(GuardianCombatModel.SetupGuardianCombatUi);
        
        FormBoneRegistry.RegisterVoidForm<Core.Guardian>("head");
        FormBoneRegistry.RegisterSerpentForm<Core.Guardian>("body");
        FormBoneRegistry.RegisterReaperForm<Core.Guardian>("head");
    }

    private static void CopyGemsToTranscendence(CardModel starter, CardModel result)
    {
        if (starter is not IGemSocketCard sourceCard) return;
        if (result is not IGemSocketCard targetCard) return;
        if (sourceCard.Gems.Count == 0) return;

        var gemClones = sourceCard.Gems
            .Take(targetCard.GemSlots)
            .Select(gem => gem.CreateClone())
            .ToList();

        targetCard.AddGems(gemClones);
    }

    private static void RegisterGemSave()
    {
        ExtendedSaveTypes.RegisterListSaveType<ModelId>();

        ExtendedSaveHandlers<CardModel, SerializableCard>.RegisterSave(
            "GuardianGems",
            card =>
            {
                var gems = CardModifier.DirectModifiers(card).OfType<GemModel>().ToList();
                return gems.Count > 0 ? [.. gems.Select(g => g.Id)] : null;
            },
            (card, gemIds) =>
            {
                if (gemIds == null) return;
                var existingGemIds =
                    CardModifier.DirectModifiers(card).OfType<GemModel>().Select(g => g.Id).ToHashSet();
                foreach (var gemId in gemIds)
                {
                    if (existingGemIds.Contains(gemId))
                        continue;
                    if (ModelDb.GetById<CardModifier>(gemId) is not GemModel canonicalGem)
                        continue;
                    var mutableGem = canonicalGem.ToMutable();
                    CardModifier.AddModifier(card, mutableGem);
                    //mutableGem.ApplyOnAddedEffects(card);
                }
            },
            (gemIds, writer) =>
            {
                if (gemIds == null)
                {
                    writer.WriteInt(0);
                    return;
                }

                writer.WriteInt(gemIds.Count);
                foreach (var id in gemIds)
                    writer.WriteModelEntry(id);
            },
            reader =>
            {
                var count = reader.ReadInt();
                if (count <= 0) return null;
                var list = new List<ModelId>(count);
                for (var i = 0; i < count; i++)
                    list.Add(reader.ReadModelIdAssumingType<CardModifier>());
                return list;
            });
    }
}
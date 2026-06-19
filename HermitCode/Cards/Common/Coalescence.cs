using System.Reflection;
using System.Reflection.Emit;
using Downfall.DownfallCode;
using Downfall.DownfallCode.Artists;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Coalescence : HermitCardModel
{
    public Coalescence() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithCards(2, 1);
        WithTip(CardKeyword.Retain);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        var hand = PileType.Hand.GetPile(Owner);
        if (hand.Cards.Count == 0) return;

        var retainable = hand.Cards.Where(c => !c.ShouldRetainThisTurn).ToList();
        if (retainable.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue);
        var selected = (await CardSelectCmd.FromHand(
            prefs: prefs,
            context: ctx,
            player: Owner,
            filter: c => !c.ShouldRetainThisTurn,
            source: this
        )).ToList();

        foreach (var card in selected) card.GiveSingleTurnRetain();
    }
}


[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand.OnSelectModeSourceFinished))]
public static class HandSelectionOrderTranspiler
{
    private static readonly MethodInfo AddMethod =
        AccessTools.Method(typeof(NPlayerHand), nameof(NPlayerHand.Add),
            [typeof(NCard), typeof(int)]);

    private static readonly MethodInfo GetHandInsertIndex =
        AccessTools.Method(typeof(NPlayerHand), nameof(NPlayerHand.GetHandInsertIndex),
            [typeof(CardModel)]);

    private static readonly MethodInfo GetModel =
        AccessTools.PropertyGetter(typeof(NCard), nameof(NCard.Model));

    private static readonly MethodInfo GetPreviewCard =
        AccessTools.PropertyGetter(typeof(NUpgradePreview), nameof(NUpgradePreview.Card));

    private static readonly FieldInfo UpgradePreviewField =
        AccessTools.Field(typeof(NPlayerHand), "_upgradePreview");

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var result = new List<CodeInstruction>(codes.Count + 8);
        var patched = 0;

        for (var i = 0; i < codes.Count; i++)
        {
            var cur = codes[i];

            var isTargetLdcM1 =
                i > 0
                && cur.opcode == OpCodes.Ldc_I4_M1
                && i + 1 < codes.Count
                && codes[i + 1].opcode == OpCodes.Call
                && (codes[i + 1].operand as MethodInfo) == AddMethod;

            if (!isTargetLdcM1)
            {
                result.Add(cur);
                continue;
            }

            var prev = codes[i - 1];
            var replacement = new List<CodeInstruction>(5);

            if (prev.IsLdloc())
            {
                replacement.Add(new CodeInstruction(OpCodes.Ldarg_0));                 // this (receiver)
                replacement.Add(new CodeInstruction(prev.opcode, prev.operand));       // reload cardNode
                replacement.Add(new CodeInstruction(OpCodes.Callvirt, GetModel));      // cardNode.Model
                replacement.Add(new CodeInstruction(OpCodes.Call, GetHandInsertIndex));
            }
            else
            {
                replacement.Add(new CodeInstruction(OpCodes.Ldarg_0));                 // this (receiver)
                replacement.Add(new CodeInstruction(OpCodes.Ldarg_0));                 // this
                replacement.Add(new CodeInstruction(OpCodes.Ldfld, UpgradePreviewField));
                replacement.Add(new CodeInstruction(OpCodes.Callvirt, GetPreviewCard));// _upgradePreview.Card
                replacement.Add(new CodeInstruction(OpCodes.Call, GetHandInsertIndex));
            }

            // Preserve any labels / exception-block markers that sat on the ldc.i4.m1.
            replacement[0].labels.AddRange(cur.labels);
            replacement[0].blocks.AddRange(cur.blocks);

            result.AddRange(replacement);
            patched++;
        }

        if (patched != 2)
            DownfallMainFile.Logger.Warn(
                $"[HandSelectionOrderTranspiler] expected to patch 2 sites, patched {patched}. " +
                "The method IL likely changed in a game update; falling back behavior is stock (cards append to end).");

        return result;
    }
}
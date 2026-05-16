using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Abstract;

public abstract class CustomIntent : AbstractIntent, ICustomModel
{
    protected override string IntentPrefix => GetType().GetPrefix() + GetType().Name.ToSnakeCase().ToUpperInvariant();
    protected override string? SpritePath => null;

    protected abstract string IntentSpritePath { get; }

    private void EnsureRegistered()
    {
        var key = IntentPrefix.ToLowerInvariant();
        if (IntentAnimData._data.ContainsKey(key)) return;
        IntentAnimData._data[key] = new IntentAnimData.InternalData
        {
            frames = [IntentSpritePath]
        };
    }

    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
    {
        EnsureRegistered();
        return base.GetAnimation(targets, owner);
    }
}

[HarmonyPatch(typeof(NIntent), nameof(NIntent.UpdateVisuals))]
internal static class CustomIntentLabelPatch
{
    private static void Postfix(NIntent __instance)
    {
        if (__instance._intent is not CustomIntent custom)
            return;

        __instance._valueLabel.Text = custom.GetIntentLabel(__instance._targets, __instance._owner)
            .GetFormattedText();
    }
}
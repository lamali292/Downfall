using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Guardian.GuardianCode.Cards.Ancient;

[Pool(typeof(GuardianCardPool))]
public class BaubleBurst : GuardianCardModel
{
    public BaubleBurst() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithRepeat(3);
    }

    public override int GemSlots => IsUpgraded ? 2 : 1;
    protected override int GemReplayCount => 3;


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
    }
}

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.GetTranscendenceTransformedCard))]
public static class TranscendenceGemCopyPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel starterCard, CardModel __result)
    {
        if (starterCard is not GuardianCardModel sourceCard) return;
        if (__result is not GuardianCardModel targetCard) return;
        if (sourceCard.Gems.Count == 0) return;

        var gemClones = sourceCard.Gems
            .Take(targetCard.GemSlots)
            .Select(gem => gem.CreateClone())
            .ToList();

        targetCard.AddGems(gemClones);
    }
}
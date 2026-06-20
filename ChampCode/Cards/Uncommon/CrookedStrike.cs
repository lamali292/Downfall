using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class CrookedStrike : ChampCardModel
{
    public CrookedStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        this.WithFinisher();
        this.WithTip<VigorPower>();
        WithTags(CardTag.Strike);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}

[HarmonyPatch(typeof(VigorPower), nameof(VigorPower.AfterAttack))]
public static class VigorPowerAfterAttackPatch
{
    private static bool Prefix(VigorPower __instance, PlayerChoiceContext choiceContext, AttackCommand command,
        ref Task __result)
    {
        if (command.ModelSource is not CardModel card) return true;
        if (card is not CrookedStrike) return true;
        __instance.GetInternalData<VigorPower.Data>().commandToModify = null;
        __result = Task.CompletedTask;
        return false;
    }
}
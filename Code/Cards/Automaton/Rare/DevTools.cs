using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils;
using BaseLib.Utils.Patching;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class DevTools : AutomatonCardModel
{
    public DevTools() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithKeywords(CardKeyword.Retain);
        WithTip(typeof(Debug));
        WithTip(typeof(Batch));
        WithTip(typeof(Decompile));
        WithTip(typeof(ByteShift));
    }
  

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var choices = new List<CardModel>
        {
            Owner.Creature.CombatState!.CreateCard<Debug>(Owner),
            Owner.Creature.CombatState!.CreateCard<Batch>(Owner),
            Owner.Creature.CombatState!.CreateCard<Decompile>(Owner),
            Owner.Creature.CombatState!.CreateCard<ByteShift>(Owner)
        };

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        if (chosen == null) return;

        var result = await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

[HarmonyPatch]
public static class FromChooseACardScreenPatch
{
    private static MethodBase TargetMethod()
    {
        // Target the MoveNext method of the async state machine
        var stateMachine = typeof(CardSelectCmd).GetNestedTypes(AccessTools.all)
            .First(t => t.Name.Contains("FromChooseACardScreen"));
        return AccessTools.Method(stateMachine, "MoveNext");
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return (List<CodeInstruction>)new InstructionPatcher(instructions)
            .Match(new InstructionMatcher()
                .opcode(OpCodes.Ldstr) // "Only works with less than 3 cards"
                .opcode(OpCodes.Ldstr) // "cards"
                .opcode(OpCodes.Newobj) // new ArgumentException
                .opcode(OpCodes.Throw))
            .ReplaceLastMatch([
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop)
            ]);
    }
}
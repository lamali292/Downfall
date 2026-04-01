using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils;
using BaseLib.Utils.Patching;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
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


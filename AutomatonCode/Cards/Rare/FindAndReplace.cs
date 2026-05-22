using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class FindAndReplace : AutomatonCardModel
{
    public FindAndReplace() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(typeof(Dazed), UpgradeType.Remove);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // get draw and discard piles
        var drawPile = PileType.Draw.GetPile(Owner);
        var discardPile = PileType.Discard.GetPile(Owner);
        var choices = drawPile.Cards.Concat(discardPile.Cards).ToList();
        if (choices.Count == 0) return;

        // Select card to move / show screen
        var selected = (await DownfallCardCmd.SelectFromCards(ctx, Owner.GetDraw(), DownfallCardSelectorPrefs.ToHandSelectionPrompt, this, optional: true)).FirstOrDefault();

        if (selected == null) return;

        // Record position before moving
        var sourcePile = selected?.Pile;
        if (selected == null || sourcePile == null) return;
        var index = sourcePile.Cards.IndexOf(selected);


        // Insert Dazed at exact index
        if (!IsUpgraded)
        {
            var dazed = Owner.Creature.CombatState!.CreateCard<Dazed>(Owner);
            var dazedResult = await CardPileCmd.AddGeneratedCardToCombat(dazed, sourcePile.Type, Owner,
                index == 0 ? CardPilePosition.Top : CardPilePosition.Bottom);
            CardCmd.PreviewCardPileAdd(dazedResult, 0.3f);
        }

        // Move to hand
        var result = await CardPileCmd.Add(selected, PileType.Hand);



        // TODO: 
        // This would be correct for ACTUAL position and not only top, bottom, random in  AddGeneratedCardToCombat.
        // But there is no CardPileCmd support for direct card replacement/insertion.
        // AddInternal maybe doesnt trigger all the hooks and events.
        /*
         var combatState = Owner.Creature.CombatState!;
        var dazed = combatState.CreateCard<Dazed>(Owner);
        CombatManager.Instance.History.CardGenerated(combatState, dazed, false);
        sourcePile.AddInternal(dazed, index);
        await Hook.AfterCardEnteredCombat(combatState, dazed);
        await Hook.AfterCardGeneratedForCombat(combatState, dazed, false);
        CardCmd.PreviewCardPileAdd(result);
        */
    }
}
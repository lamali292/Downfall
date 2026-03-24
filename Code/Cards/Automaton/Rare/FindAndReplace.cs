using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class FindAndReplace() : AutomatonCardModel(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // get draw and discard piles
        var drawPile = PileType.Draw.GetPile(Owner);
        var discardPile = PileType.Discard.GetPile(Owner);
        var choices = drawPile.Cards.Concat(discardPile.Cards).ToList();
        if (choices.Count == 0) return;

        // Select card to move / show screen
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var screen = NSimpleCardSelectScreen.Create(choices, prefs);
        NOverlayStack.Instance?.Push(screen);
        var selected = (await screen.CardsSelected()).FirstOrDefault();

        // Record position before moving
        var sourcePile = selected?.Pile;
        if (selected == null || sourcePile == null) return;
        var index = sourcePile.Cards.IndexOf(selected);


        // Insert Dazed at exact index
        var dazed = Owner.Creature.CombatState!.CreateCard<Dazed>(Owner);
        var dazedResult = await CardPileCmd.AddGeneratedCardToCombat(dazed, sourcePile.Type, false,
            index == 0 ? CardPilePosition.Top : CardPilePosition.Bottom);
        // Move to hand
        var result = await CardPileCmd.Add(selected, PileType.Hand);

        // Animations
        CardCmd.PreviewCardPileAdd(dazedResult, 0.3f);
        CardCmd.PreviewCardPileAdd(result, 0.3f);


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
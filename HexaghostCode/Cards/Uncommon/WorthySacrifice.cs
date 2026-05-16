using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class WorthySacrifice : HexaghostCardModel
{
    public WorthySacrifice() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        var cards = (await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e != this, this)).ToList();
        foreach (var card in cards) await CardCmd.Exhaust(ctx, card);

        await TransformCards(cards, CardType.Attack, CardType.Skill);
        await TransformCards(cards, CardType.Skill, CardType.Attack);
    }

    private async Task TransformCards(List<CardModel> cards, CardType from, CardType to)
    {
        var count = cards.Count(e => e.Type == from);
        if (count == 0) return;

        var pool = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == to);

        var newCards = CardFactory.GetDistinctForCombat(Owner, pool, count, Owner.RunState.Rng.CombatCardGeneration)
            .ToList();
        if (IsUpgraded)
            foreach (var card in newCards)
                card.UpgradeInternal();
        await CardPileCmd.AddGeneratedCardsToCombat(newCards, PileType.Hand, Owner);
    }
}
using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ArenaPreparation : ChampCardModel
{
    public ArenaPreparation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(CardKeyword.Retain);
        WithCards(2);
        WithCostUpgradeBy(-1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var list = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Type == CardType.Skill)
            .ToList();
        if (list.Count <= 0)
            return;
        var combatCardGeneration = Owner.RunState.Rng.CombatCardGeneration;
        var a = CardFactory.GetDistinctForCombat(Owner, list, DynamicVars.Cards.IntValue, combatCardGeneration)
            .ToList();
        foreach (var cardModel in a) CardCmd.ApplyKeyword(cardModel, CardKeyword.Retain);
        var result = await CardPileCmd.AddGeneratedCardsToCombat(a, PileType.Hand, Owner);
    }
}
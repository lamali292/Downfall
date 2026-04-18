using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ArenaPreparation : ChampCardModel
{
    public ArenaPreparation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCards(2);
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
        foreach (var cardModel in a) CardCmd.ApplyKeyword(cardModel, CardKeyword.Ethereal);
        var result = await CardPileCmd.AddGeneratedCardsToCombat(a, PileType.Hand, true);
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
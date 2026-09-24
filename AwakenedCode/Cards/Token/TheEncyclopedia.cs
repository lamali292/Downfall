using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class TheEncyclopedia : AwakenedCardModel
{
    public TheEncyclopedia() : base(2, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCards(4, 2);
    }

    public override Texture2D? CustomFrame =>
        ResourceLoader.Load<Texture2D>("res://Awakened/images/dimension/obelisk_skill.png");

    public override Material? CreateCustomFrameMaterial => ShaderUtils.GenerateHsv(1, 1, 1);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = CardFactory.GetDistinctForCombat(Owner, Owner.Character.CardPool.AllCards, DynamicVars.Cards.IntValue,
                Owner.RunState.Rng.CombatCardGeneration)
            .Select(e => new CardCreationResult(e)).ToList();
        var card = (await CardSelectCmd.FromSimpleGridForRewards(ctx, cards, Owner,
            new CardSelectorPrefs(DownfallCardSelectorPrefs.ToHandSelectionPrompt, 2, 2))).ToList();

        foreach (var cardModel in card) cardModel.EnergyCost.AddThisCombat(-2);
        await CardPileCmd.AddGeneratedCardsToCombat(card, PileType.Hand, Owner);
    }
}
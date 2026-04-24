using BaseLib.Abstracts;
using Downfall.Code.Character;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.Code.Abstract.CardModels;

public abstract class HexaghostCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Hexaghost>(cost, type, rarity, targetType)
{
    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected ConstructedCardModel WithAfterlife()
    {
        return WithKeywords(CardKeyword.Ethereal, DownfallKeywords.Afterlife);
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
    }

    protected virtual Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay) => Task.CompletedTask;
    
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this || CombatState == null || !card.Keywords.Contains(DownfallKeywords.Afterlife)) return;
        var a = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        var cardPlay = new CardPlay
        {
            Card = card,
            Target = a,
            ResultPile = PileType.Exhaust,
            Resources = default,
            IsAutoPlay = true,
            PlayIndex = 0,
            PlayCount = 0
        };
        await AfterlifeEffect(ctx, cardPlay);
    }
}
using BaseLib.Abstracts;
using Downfall.Code.Character;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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

    protected virtual Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay = null) => Task.CompletedTask;
    
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        await AfterlifeEffect(ctx);
    }
}
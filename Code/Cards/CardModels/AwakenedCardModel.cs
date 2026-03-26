using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using AwakenedCharacter = Downfall.Code.Character.Awakened;

namespace Downfall.Code.Cards.CardModels;

public abstract class AwakenedCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : CustomCardModel(cost, type, rarity, targetType)
{
    public sealed override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<AwakenedCharacter>();

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
    }
}
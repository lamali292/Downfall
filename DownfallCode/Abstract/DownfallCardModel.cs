using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallCardModel
    : ConstructedCardModel
{
    protected DownfallCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(e => e is DownfallCardModel { Artist: not null } card ? [card.Artist.HoverTip] : []);
    }

    protected virtual Artist? Artist => null;

    protected virtual Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
    
    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (await CardExecutionRegistry.BeforeOnPlayInternal(this, ctx, cardPlay)) return;
        await OnPlayInternal(ctx, cardPlay);
        await CardExecutionRegistry.AfterOnPlayInternal(this, ctx, cardPlay);
    }
}

public abstract class DownfallCardModel<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    where T : DownfallCharacterModel
{
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".CardImageAtlasPath<T>();
}
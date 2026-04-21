using Downfall.Code.Character;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Abstract.CardModels;

public abstract class ChampCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Champ>(cost, type, rarity, targetType)
{
    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        if (Keywords.Contains(DownfallKeywords.TriggerSkillBonus)) await Owner.ChampStance().SkillBonus();
    }
}
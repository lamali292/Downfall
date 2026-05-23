using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.DynamicVars;

namespace SlimeBoss.SlimeBossCode.Cards;

public abstract class SlimeBossCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.SlimeBoss>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }


    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
    }

    protected ConstructedCardModel WithSlurp(decimal baseVal, decimal upgradedVal = 0)
    {
        WithVar(new SlurpVar(baseVal).WithUpgrade(upgradedVal));
        return this;
    }

    protected ConstructedCardModel WithCommand(decimal baseVal, decimal upgradedVal = 0)
    {
        WithVar(new CommandVar(baseVal).WithUpgrade(upgradedVal));
        return this;
    }
}
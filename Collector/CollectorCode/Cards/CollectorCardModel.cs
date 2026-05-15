using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards;

public abstract class CollectorCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Core.Collector>(cost, type, rarity, targetType)
{
    private bool _hasPyre;
    public virtual bool UsesCollectorEnergyOnly => false;
    protected CardModel? PyredCard { get; private set; }

    protected override bool IsPlayable => !_hasPyre || PyreCondition();

    protected CollectorCardModel WithPyre()
    {
        _hasPyre = true;
        WithKeyword(CollectorKeyword.Pyre);
        return this;
    }

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (_hasPyre)
        {
            PyredCard = await CollectorCmd.Pyre(ctx, this);
            if (PyredCard == null) return;
        }

        await PlayEffect(ctx, cardPlay);
    }

    private bool PyreCondition()
    {
        return PileType.Hand.GetPile(Owner).Cards.Any(e => e != this);
    }
}
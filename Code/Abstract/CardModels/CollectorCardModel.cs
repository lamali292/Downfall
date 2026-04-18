using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract.CardModels;

public abstract class CollectorCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Character.Collector>(cost, type, rarity, targetType)
{
    public virtual bool UsesCollectorEnergyOnly => false;
    private bool _hasPyre;
    protected CardModel? PyredCard { get; private set; }
    
    protected CollectorCardModel WithPyre()
    {
        _hasPyre = true;
        WithKeyword(DownfallKeywords.Pyre);
        return this;
    }

    protected override bool IsPlayable => !_hasPyre || PyreCondition();
    
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
    
    protected bool PyreCondition() => PileType.Hand.GetPile(Owner).Cards.Any(e => e != this);
}
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Stockpile : SneckoCardModel, IHasOverflowEffect
{
    public Stockpile() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithOverflow();
        WithKeyword(CardKeyword.Exhaust);
        WithCards(7, 1);
        WithEnergy(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var baseValue = DynamicVars.Cards.BaseValue;
        var count = Owner.GetHand().Count;
        await CardPileCmd.Draw(ctx, Math.Max(0M, baseValue - count), Owner);
    }

    public Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
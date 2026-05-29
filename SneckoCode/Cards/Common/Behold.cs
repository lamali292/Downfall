using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Behold : SneckoCardModel, IHasOverflowEffect
{
    public Behold() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        this.WithOverflow();
        WithDamage(6, 3);
        WithCards(2);
        this.WithTip<Shiv>();
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, DynamicVars.Cards.BaseValue);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
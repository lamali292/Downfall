using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards.Ancient;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;

namespace Snecko.SneckoCode.Cards.Basic;

[Pool(typeof(SneckoCardPool))]
public class SnekBite : SneckoCardModel
{
    public SnekBite() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        this.WithMuddle(1, 1);
    }

 
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}
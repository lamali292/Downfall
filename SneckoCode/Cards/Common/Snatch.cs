using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Snatch : SneckoCardModel, IHasOverflowEffect
{
    public Snatch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        this.WithOverflow();
        WithCards(1, 1);
        WithVar(new CardsVar("OverflowCards", 1));
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
         await CommonActions.Draw(this, ctx);
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, DynamicVars["OverflowCards"].BaseValue, Owner);
    }
}
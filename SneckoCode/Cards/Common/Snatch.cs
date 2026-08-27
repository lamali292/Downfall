using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

//TODO : remove or rework so we have 20 commons again
[Pool(typeof(SneckoCardPool))]
[Obsolete]
public class Snatch : SneckoCardModel, IHasOverflowEffect
{
    public Snatch() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, false)
    {
        WithOverflow();
        WithCards(1, 1);
        WithVar(new CardsVar("OverflowCards", 1));
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, DynamicVars["OverflowCards"].BaseValue, Owner);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
    }
}
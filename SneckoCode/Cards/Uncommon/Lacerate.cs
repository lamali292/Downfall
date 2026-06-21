using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Lacerate : SneckoCardModel
{
    public Lacerate() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<VenomPower>(4);
        WithUpgradingCardTip<Deathbringer>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<VenomPower>(ctx, this, cardPlay);
        await DownfallCardCmd.GiveCard<Deathbringer>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Cower : SneckoCardModel
{
    public Cower() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(14, 4);
        WithUpgradingCardTip<HoleUp>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await DownfallCardCmd.GiveCard<HoleUp>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }
}
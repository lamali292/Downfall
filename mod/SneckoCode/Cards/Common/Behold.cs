using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Behold : SneckoCardModel
{
    public Behold() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithOverflow();
        WithDamage(6, 3);
        WithCards(2);
        WithTip<Shiv>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await OverflowCmd.Overflow(overflowing, cardPlay,
            () => DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, DynamicVars.Cards.BaseValue));
    }
}
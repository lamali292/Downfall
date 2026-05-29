using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class ToothAndClaw : SneckoCardModel, IHasGift
{
    public ToothAndClaw() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithDamage(4, 2);
        WithUpgradingCardTip<Shiv>();
    }

    private int UniqueColorsInHand => Owner.GetHand()
        .Select(e => e.Pool)
        .Distinct()
        .Count();

    public Gift? Gift { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, UniqueColorsInHand, upgraded: IsUpgraded);
    }
}
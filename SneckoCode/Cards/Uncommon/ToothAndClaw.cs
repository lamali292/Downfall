using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class ToothAndClaw : SneckoCardModel
{
    public ToothAndClaw() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithDamage(4, 2);
        WithUpgradingCardTip<Shiv>();
    }

    private int UniqueColorsInHand => Owner.PlayerCombatState?
        .Hand.Cards
        .Select(e => e.Pool)
        .Distinct()
        .Count() ?? 0;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, UniqueColorsInHand, upgraded: IsUpgraded);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class ToothAndClaw : SneckoCardModel, IHasGift
{
    public ToothAndClaw() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithDamage(4, 2);
        WithUpgradingCardTip<Shiv>();
        WithCalculatedVar("Shivs", 0, UniqueColorsInHand);
    }

    // Excludes this card: it has left the hand by the time it resolves, so the preview matches the result.
    private static decimal UniqueColorsInHand(CardModel card, Creature? _)
    {
        return card.Owner.Hand
            .Where(e => e != card)
            .Select(e => e.VisualCardPool)
            .Distinct()
            .Count();
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var shivs = (int)((CalculatedVar)DynamicVars["Shivs"]).Calculate(cardPlay.Target);
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, shivs, upgraded: IsUpgraded);
    }
}
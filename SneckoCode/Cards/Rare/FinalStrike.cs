using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class FinalStrike : SneckoCardModel, IHasGift
{
    public FinalStrike() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            IsStrike = true
        });
        WithTags(CardTag.Strike);
        WithDamage(6, 3);
        WithCalculatedVar("UniqueStrikesPlayed", 0, UniqueStrikesPlayed);
    }

    public Gift? Gift { get; set; }

    private static decimal UniqueStrikesPlayed(CardModel card, Creature? creature)
    {
        return CombatManager.Instance.History.CardPlaysFinished
            .Select(e => e.CardPlay.Card)
            .Where(e => e.Owner == card.Owner && e.Tags.Contains(CardTag.Strike))
            .DistinctBy(c => c.Id)
            .Count() + 1;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = (int)UniqueStrikesPlayed(this, null);
        await CommonActions.CardAttack(this, cardPlay, repeat).Execute(ctx);
    }
}
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class FinalStrike : SneckoCardModel
{
    public FinalStrike() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
           IsStrike = true
        });
        WithTags(CardTag.Strike);
        WithDamage(6, 3);
    }
    
    private int UniqueStrikesPlayed => CombatManager.Instance.History.CardPlaysFinished
        .Select(e => e.CardPlay.Card)
        .Where(e => e.Owner == Owner && e.Tags.Contains(CardTag.Strike))
        .DistinctBy(c => c.Id)
        .Count();
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 1+UniqueStrikesPlayed).Execute(ctx);
    }
}
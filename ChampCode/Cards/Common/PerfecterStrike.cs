using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class PerfecterStrike : ChampCardModel
{
    public PerfecterStrike() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithCalculatedDamage(8, 2, CalculateStrikeCount, bonusUpgrade: 1);
    }

    private static decimal CalculateStrikeCount(CardModel card, Creature? creatures)
    {
        return card.Owner.PlayerCombatState == null
            ? 0M
            : card.Owner.PlayerCombatState.AllCards.Count(c => c.Tags.Contains(CardTag.Strike));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
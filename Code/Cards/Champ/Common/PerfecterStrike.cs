using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class PerfecterStrike : ChampCardModel
{
    public PerfecterStrike() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithCalculatedDamage(8, CalculateStrikeCount);
        WithVars(new ExtraDamageVar(2).WithUpgrade(1));
    }
    
    private static decimal CalculateStrikeCount(CardModel card, Creature? creatures)
    {
        if (card.Owner.PlayerCombatState == null) return 0M;
        return card.Owner.PlayerCombatState.AllCards.Count(
            c => c.Tags.Contains(CardTag.Strike));
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
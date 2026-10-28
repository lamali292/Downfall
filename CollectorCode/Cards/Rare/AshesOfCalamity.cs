using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class AshesOfCalamity : CollectorCardModel
{
    public AshesOfCalamity() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithCalculatedBlock(9, 3, CalcBlock, BlockProps.card, 3, 0);
        WithVar("Increase", 3, 1);
    }
    
    private static decimal CalcBlock(CardModel card, Creature? creature)
    {
        return card.Owner.PlayerCombatState?.AllCards.Count(c => c.Type == CardType.Status) ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
    
   

}
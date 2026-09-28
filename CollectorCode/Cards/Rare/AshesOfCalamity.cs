using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
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
        WithCalculatedBlock(9, 3, CalcBlock, BlockProps.card, 3, 0);
        WithVar("Increase", 3, 1);
        WithTip(CardKeyword.Ethereal);
    }
    
    private static decimal CalcBlock(CardModel card, Creature? creature)
    {
        var pileA = card.Owner.Hand.Count(c => c.Type == CardType.Status);
        var pileB = card.Owner.DrawPile.Count(c => c.Type == CardType.Status);
        var pileC = card.Owner.DiscardPile.Count(c => c.Type == CardType.Status);
        var pileD = card.Owner.ExhaustPile.Count(c => c.Type == CardType.Status);
        return (pileA + pileB + pileC + pileD);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var statusCards = Owner.ExhaustPile.Where(e => e.Type == CardType.Status).ToList();
        if (statusCards.Count >= DynamicVars["Increase"].IntValue)
        {
            await CardRemovalCmd.RemoveFromCombat(statusCards);
        }
        else
        {
            await CardCmdCompatibility.Exhaust(ctx, this);
        }
    }
    
   

}
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class AshesOfCalamity : CollectorCardModel
{
    public AshesOfCalamity() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(22, 8);
        WithTip(CardKeyword.Exhaust);
        WithVar("Threshold", 5);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var statusCards = Owner.ExhaustPile.Where(e => e.Type == CardType.Status).ToList();
        if (statusCards.Count >= DynamicVars["Threshold"].IntValue)
        {
            await DownfallCardCmd.RemoveFromCombat(statusCards);
        }
        else
        {
            await CardCmdCompatibility.Exhaust(ctx, this);
        }
    }
    
   

}
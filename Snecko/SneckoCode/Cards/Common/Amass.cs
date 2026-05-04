using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Amass : SneckoCardModel
{
    public Amass() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCalculatedBlock(12, Calc, ValueProp.Move, 4);
    }

    private static decimal Calc(CardModel card, Creature? creature) => 
        card.Owner.PlayerCombatState?.Hand.Cards.Sum(e => e.EnergyCost.GetResolved()) ?? 0;
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.CardCalculatedBlock(this, cardPlay);
    }
}
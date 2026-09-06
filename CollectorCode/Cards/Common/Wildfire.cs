using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Wildfire : CollectorCardModel
{
    public Wildfire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithCalculatedVar("Hits", 1, RepeatCalc);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    private static decimal RepeatCalc(CardModel card, Creature? creature) => creature?.Powers.Count(ShouldCountPower) >= 2 ? 1 : 0;
    
    private static bool ShouldCountPower(PowerModel power)
    {
        return power.TypeForCurrentAmount == PowerType.Debuff && power is not ITemporaryPower;
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = (int)((CalculatedVar)DynamicVars["Hits"]).Calculate(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay, repeat).Execute(ctx);
    }
}
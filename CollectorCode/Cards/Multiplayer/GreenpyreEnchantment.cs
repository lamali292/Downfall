using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Multiplayer;

[Pool(typeof(CollectorCardPool))]
public class GreenpyreEnchantment : CollectorCardModel
{
    public GreenpyreEnchantment() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithPower<GreenpyreEnchantmentPower>(1, 1, false);
        WithTip<BlisterPower>();
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GreenpyreEnchantmentPower>(ctx, this, cardPlay);
    }
}
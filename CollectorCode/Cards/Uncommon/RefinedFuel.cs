using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RefinedFuel : CollectorCardModel
{
    public RefinedFuel() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithReserve(1);
        WithTip(CollectorTip.Pyred);
        WithPower<RefinedFuelPower>(1, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<RefinedFuelPower>(ctx, this);
    }
}
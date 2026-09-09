using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;


public class MechaKnightCard : Collectible<MechaKnightElite>
{
    public MechaKnightCard() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0.63f)
    {
        WithTip(StaticHoverTip.Channeling);
        WithTip(CollectorTip.Pyred);
        WithTip(CardKeyword.Exhaust);
        WithPower<MechaKnightCardPower>(1, false);
        WithCostUpgradeBy(-1);
        WithVar("OrbSlots", 10);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await OrbCmd.AddSlots(Owner, DynamicVars["OrbSlots"].IntValue);
        await CommonActions.ApplySelf<MechaKnightCardPower>(ctx, this);
    }
}

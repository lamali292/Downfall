using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Cards.Rare;


[Pool(typeof(CollectorCardPool))]
public class EquipShield : CollectorCardModel
{
    public EquipShield() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<EquipShieldPower>(2, 3, false);
        WithKindle(6, 2);
        WithTip(StaticHoverTip.Block);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CollectorCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<EquipShieldPower>(ctx, this);
    }
}
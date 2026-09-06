using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class EquipStaff : CollectorCardModel
{
    public EquipStaff() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKindle(6, 2);
        WithPower<EquipStaffPower>(3, 2, false);
        WithTip<MiasmaPower>();
    }
    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CollectorCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<EquipStaffPower>(ctx, this);
    }
}
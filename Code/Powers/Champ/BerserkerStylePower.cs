using Downfall.Code.Abstract;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Champ;

public class BerserkerStylePower : ChampPowerModel, IModifySkillBonus
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>()
    ];
    
    public int ModifySkillBonus<TPower>(ChampStanceModel stance, int amount)
        where TPower : PowerModel
    {
        if (typeof(TPower) != typeof(VigorPower) || stance.Owner.Creature != Owner) return amount;
        return amount + Amount;
    }
}
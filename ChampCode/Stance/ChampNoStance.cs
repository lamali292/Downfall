using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using MegaCrit.Sts2.Core.HoverTips;

namespace Champ.ChampCode.Stance;

public class ChampNoStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => false;
    public override bool HasFinisher => false;
}
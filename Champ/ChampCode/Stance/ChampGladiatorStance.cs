using Champ.ChampCode.Core;

namespace Champ.ChampCode.Stance;

public class ChampGladiatorStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Champ/images/ui/stance_charge_inactive.png";
}
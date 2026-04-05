namespace Downfall.Code.Core.Champ;

public class GladiatorChampStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Downfall/images/ui/stance_charge_active.png";
}
namespace Downfall.Code.Core.Champ;

public class NoChampStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => false;
    public override bool HasFinisher => false;
}

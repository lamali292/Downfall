namespace Downfall.Code.Core.Champ;

public class ChampNoStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => false;
    public override bool HasFinisher => false;
}
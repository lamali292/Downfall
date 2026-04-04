namespace Downfall.Code.Core.Champ;

public class NoneStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => false;
    public override bool HasFinisher => false;
}

using Champ.ChampCode.Core;

namespace Champ.ChampCode.Stance;

public class ChampNoStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => false;
    public override bool HasFinisher => false;
}
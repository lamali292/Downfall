using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Compile;

public class BurnToDrawCompile() : CardToPileCompile<Burn>(PileType.Draw, "CompileBurn")
{
    public override string Id => "BURN_TO_DRAW_COMPILE";
    public override int Order => 3;
}

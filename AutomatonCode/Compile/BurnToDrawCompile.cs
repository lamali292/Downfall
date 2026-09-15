using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Compile;

public class BurnToDrawCompile() : CardToPileCompile<Burn>(PileType.Draw, "CompileBurn");

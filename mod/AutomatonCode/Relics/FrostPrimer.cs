using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class FrostPrimer : AutomatonRelicModel, IModifyCompiledFunction
{
    public FrostPrimer() : base(RelicRarity.Rare)
    {
        WithTip<Steady>();
    }

    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (player != Owner) return false;
        CardCmd.Enchant<Steady>(function, 1);
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        return Task.CompletedTask;
    }
}
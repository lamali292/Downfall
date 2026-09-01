using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class NullPointer : AutomatonCardModel,
    IEncodable
{
    public NullPointer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithBlock(10, 3);
        WithEnergy(3);
    }

    public void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
        function.EnergyCost.SetCustomBaseCost(DynamicVars.Energy.IntValue);
    }

    public IEnumerable<Encodable> Encodings => [new BlockEncode(), new DamageEncode()];
}
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class NullPointer : AutomatonCardModel,
    IEncodable
{
    public NullPointer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTip(CardKeyword.Unplayable);
        WithDamage(12, 3);
        WithBlock(12, 3);
    }
    
    public void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
        function.AddKeyword(CardKeyword.Unplayable);
    }

    public IEnumerable<Encodable> Encodings => [new BlockEncode(), new DamageEncode()];
}
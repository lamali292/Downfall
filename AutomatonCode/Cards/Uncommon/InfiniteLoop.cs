using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteLoop : AutomatonCardModel,
    IEncodable, ICompilable
{
    public InfiniteLoop() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVar("Increase", 2, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public IEnumerable<Compilable> Compilations => [new InfiniteLoopCompile()];
    public IEnumerable<Encodable> Encodings => [new DamageEncode()];
}
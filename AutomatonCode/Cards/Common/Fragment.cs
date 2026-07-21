using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Fragment : AutomatonCardModel,
    IEncodable
{
    public Fragment() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(3, 1);
        WithDamage(3, 1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    public IEnumerable<Encodable> Encodings => [new BlockEncode(), new DamageEncode()];
}
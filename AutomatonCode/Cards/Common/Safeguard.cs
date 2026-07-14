using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Safeguard : AutomatonCardModel, IEncodable
{
    public Safeguard() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4, 2);
    }
    
    public IEnumerable<Encodable> Encodings => [new BlockEncode()];
    protected override Artist Artist => Artist.Get<Opal>();
    
}
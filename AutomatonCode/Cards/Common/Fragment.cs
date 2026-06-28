using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Fragment() : AutomatonCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy),
    IEncodable<FragmentEncode>
{
    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();
}
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class BuggyMess : AutomatonCardModel, IEncodable
{
    public BuggyMess() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEnergyTip();
        WithTip<Dazed>();
        WithCostUpgradeBy(-1);
        WithEnergy(1);
        WithVar("Dazed", 1);
    }

    public IEnumerable<Encodable> Encodings => [new EnergyEncode(), new DazedEncode()];
}
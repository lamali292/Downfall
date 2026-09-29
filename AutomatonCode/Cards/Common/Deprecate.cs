using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Deprecate : AutomatonCardModel
{
    public Deprecate() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithEncode<WeakEncode>();
        WithPower<WeakPower>(1, 1);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

}
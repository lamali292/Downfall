using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Invalidate : AutomatonCardModel
{
    public Invalidate() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithEncode<VulnerableEncode>();
        WithPower<VulnerablePower>(1, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();
}
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class FullRelease : AutomatonCardModel
{
    public FullRelease() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithEncode<PowerEncode>();
        WithCostUpgradeBy(-1);
        WithPower<FullReleasePower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();
}
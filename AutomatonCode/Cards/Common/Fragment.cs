using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Fragment : AutomatonCardModel
{
    public Fragment() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithEncode<BlockEncode>();
        WithEncode<DamageEncode>();
        WithBlock(3, 1);
        WithDamage(3, 1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

}
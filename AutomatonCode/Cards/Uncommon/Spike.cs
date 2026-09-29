using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Spike : AutomatonCardModel
{
    public Spike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithCompile<ThornsCompile>();
        WithPower<ThornsPower>(3, 2);
        WithDamage(7, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

}
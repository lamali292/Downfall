using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class DigitalCarnage : AutomatonCardModel,
    IEncodable
{
    public DigitalCarnage() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(20, 8);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public IEnumerable<Encodable> Encodings => [new DamageEncode()];
}
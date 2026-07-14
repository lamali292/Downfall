using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class MinorBeam : AutomatonCardModel, IEncodable
{
    public MinorBeam() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
    }

    public IEnumerable<Encodable> Encodings => [new DamageEncode()];
    protected override Artist Artist => Artist.Get<Opal>();
}
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class MinorBeam : AutomatonCardModel
{
    public MinorBeam() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithDamage(6, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

}
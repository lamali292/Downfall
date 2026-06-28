using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class MinorBeam() : AutomatonCardModel(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy),
    IEncodable<MinorBeamEncode>
{
    protected override Artist Artist => Artist.Get<Opal>();
}
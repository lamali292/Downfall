using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class DigitalCarnage : AutomatonCardModel,
    IEncodable<DigitalCarnageEncode>
{
    public DigitalCarnage() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
}
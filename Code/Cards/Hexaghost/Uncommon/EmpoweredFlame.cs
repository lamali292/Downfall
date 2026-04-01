using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class EmpoweredFlame : HexaghostCardModel
{
    public EmpoweredFlame() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
    }
    // TODO: Implement
}
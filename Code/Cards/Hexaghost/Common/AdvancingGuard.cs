using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Hexaghost.Common;

[Pool(typeof(HexaghostCardPool))]
public class AdvancingGuard : HexaghostCardModel
{
    public AdvancingGuard() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Rare;

[Pool(typeof(GremlinsCardPool))]
public class TargetWounds : GremlinsCardModel
{
    public TargetWounds() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}
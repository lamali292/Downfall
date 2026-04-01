using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class GemFinder : GuardianCardModel
{
    public GemFinder() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}
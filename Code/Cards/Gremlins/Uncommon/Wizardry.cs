using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Wizardry : GremlinsCardModel
{
    public Wizardry() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
    }
    // TODO: Implement
}
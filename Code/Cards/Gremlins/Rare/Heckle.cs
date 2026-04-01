using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Rare;

[Pool(typeof(GremlinsCardPool))]
public class Heckle : GremlinsCardModel
{
    public Heckle() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}
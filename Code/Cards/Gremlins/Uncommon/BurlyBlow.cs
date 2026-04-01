using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class BurlyBlow : GremlinsCardModel
{
    public BurlyBlow() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}
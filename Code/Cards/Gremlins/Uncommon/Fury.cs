using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Fury : GremlinsCardModel
{
    public Fury() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}
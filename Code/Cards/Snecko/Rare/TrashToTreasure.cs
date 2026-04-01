using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Rare;

[Pool(typeof(SneckoCardPool))]
public class TrashToTreasure : SneckoCardModel
{
    public TrashToTreasure() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
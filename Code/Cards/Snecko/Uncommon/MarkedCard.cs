using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MarkedCard : SneckoCardModel
{
    public MarkedCard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
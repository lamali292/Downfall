using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class FlashInThePan : SneckoCardModel
{
    public FlashInThePan() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Rare;

[Pool(typeof(SneckoCardPool))]
public class SoulExchange : SneckoCardModel
{
    public SoulExchange() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
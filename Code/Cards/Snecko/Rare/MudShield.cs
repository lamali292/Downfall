using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Rare;

[Pool(typeof(SneckoCardPool))]
public class MudShield : SneckoCardModel
{
    public MudShield() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}
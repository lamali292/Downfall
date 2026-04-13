using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Snecko.Token;

[Pool(typeof(TokenCardPool))]
public class SoulRoll : SneckoCardModel
{
    public SoulRoll() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
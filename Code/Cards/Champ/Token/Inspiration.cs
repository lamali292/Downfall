using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Champ.Token;

[Pool(typeof(TokenCardPool))]
public class Inspiration : ChampCardModel
{
    public Inspiration() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        
    }
    // TODO: Implement
}
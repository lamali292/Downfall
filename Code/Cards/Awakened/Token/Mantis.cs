using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Mantis : AwakenedCardModel
{
    public Mantis() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }
    // TODO: Implement
}
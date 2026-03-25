using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class DefendChamp() : ChampCardModel(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    // TODO: Implement
}
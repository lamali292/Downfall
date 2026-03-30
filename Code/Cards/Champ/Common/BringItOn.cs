using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class BringItOn() : ChampCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // TODO: Implement
}
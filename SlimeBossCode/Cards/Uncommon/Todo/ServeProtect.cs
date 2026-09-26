using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class ServeProtect() : SlimeBossCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, false, false);
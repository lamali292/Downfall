using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class WasteNot() : SlimeBossCardModel(2, CardType.Skill, CardRarity.Rare, TargetType.Self, false, false);
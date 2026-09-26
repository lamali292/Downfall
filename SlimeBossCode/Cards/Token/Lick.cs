using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
[Obsolete]
public class Lick() : SlimeBossCardModel(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy);
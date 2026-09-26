using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class Liquidate() : SlimeBossCardModel(0, CardType.Skill, CardRarity.Rare, TargetType.Self, false, false)
{
    protected override Artist Artist => Artist.Get<Opal>();
}
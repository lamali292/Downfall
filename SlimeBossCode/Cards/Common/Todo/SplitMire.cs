using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class SplitMire() : SlimeBossCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self, false, false)
{
    protected override Artist Artist => Artist.Get<Opal>();
}
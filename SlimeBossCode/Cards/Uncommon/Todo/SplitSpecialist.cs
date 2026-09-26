using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class SplitSpecialist()
    : SlimeBossCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, false, false)
{
    protected override Artist Artist => Artist.Get<Opal>();
    
}
using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class SlimeSpikes() : SlimeBossCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self, false, false)
{
    protected override Artist Artist => Artist.Get<HalfGoblinHankins>();
}

[Obsolete]
public class SlimeSpikesPower : CustomTemporaryPowerModelWrapper<SlimeSpikes, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}
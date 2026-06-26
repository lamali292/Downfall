using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeSpikes : SlimeBossCardModel
{
    public SlimeSpikes() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        this.WithPower<SlimeSpikesPower>(3, 1, false);
        this.WithTip<ThornsPower>();
    }

    protected override Artist Artist => Artist.Get<HalfGoblinHankins>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<SlimeSpikesPower>(ctx, this);
    }
}

public class SlimeSpikesPower : CustomTemporaryPowerModelWrapper<SlimeSpikes, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}
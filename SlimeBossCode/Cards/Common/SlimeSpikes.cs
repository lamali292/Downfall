using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeSpikes : SlimeBossCardModel
{
    public SlimeSpikes() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        WithPower<SlimeSpikesPower>(4, 2, false);
        WithTip<ThornsPower>();
    }

    protected override bool ShouldGlowGoldInternal => SlimeBossCmd.HasPlayedStatusThisTurn(Owner);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var playedStatus = SlimeBossCmd.HasPlayedStatusThisTurn(Owner);
        await CommonActions.CardBlock(this, cardPlay);
        if (playedStatus) await CommonActions.ApplySelf<SlimeSpikesPower>(ctx, this);
    }
}

public class SlimeSpikesPower : CustomTemporaryPowerModelWrapper<SlimeSpikes, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}

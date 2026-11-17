using BaseLib.Abstracts;
using BaseLib.Utils;
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
        WithBlock(7, 3);
        WithPower<ThornsPower>(2);
    }

    protected override bool ShouldGlowGoldInternal => SlimeBossCmd.HasPlayedStatusThisTurn(Owner);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var playedStatus = SlimeBossCmd.HasPlayedStatusThisTurn(Owner);
        await CommonActions.CardBlock(this, cardPlay);
        if (playedStatus) await CommonActions.ApplySelf<ThornsPower>(ctx, this);
    }
}

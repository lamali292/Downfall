using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class RallyingCry : SlimeBossCardModel
{
    public RallyingCry() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithPower<RallyingCryPotencyPower>(3, false);
        WithCommand(0, 1);
        WithSlimeTip<BruiserSlime>();
        WithTip<PotencyPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RallyingCryPotencyPower>(ctx, this);
        await SlimeBossCmd.Command<BruiserSlime>(ctx, this);
    }
}

public class RallyingCryPotencyPower : CustomTemporaryPowerModelWrapper<RallyingCry, PotencyPower>;

using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Shakedown : SlimeBossCardModel
{
    public Shakedown() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<ShakedownPotencyPower>(1, 1, false);
        WithTip<PotencyPower>();
        WithSlimeTip<BruiserSlime>();
        WithCommand(2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ShakedownPotencyPower>(ctx, this);
        await SlimeBossCmd.Command<BruiserSlime>(ctx, this);
    }
}

public class ShakedownPotencyPower : CustomTemporaryPowerModelWrapper<Shakedown, PotencyPower>;

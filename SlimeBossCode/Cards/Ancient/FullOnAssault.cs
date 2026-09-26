using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Ancient;

[Pool(typeof(SlimeBossCardPool))]
public class FullOnAssault : SlimeBossCardModel
{
    public FullOnAssault() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<FullOnAssaultPotencyPower>(2, false);
        WithTip<PotencyPower>();
        WithTip(SlimeBossTip.Command);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FullOnAssaultPotencyPower>(ctx, this);
        await SlimeBossCmd.CommandAll(ctx, Owner, 1, this);
    }
}

public class FullOnAssaultPotencyPower : CustomTemporaryPowerModelWrapper<FullOnAssault, PotencyPower>;

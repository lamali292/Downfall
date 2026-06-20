using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Overexert : SlimeBossCardModel
{
    public Overexert() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<PotencyPower>(5);
        this.WithCommand(0, 2);
        this.WithPower<OverexertPower>(2, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PotencyPower>(ctx, this);
        await SlimeBossCmd.Command(ctx, this);
        await CommonActions.ApplySelf<OverexertPower>(ctx, this);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeSlap : SlimeBossCardModel
{
    public SlimeSlap() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithPower<DouseInSlimePower>(1, false);
        WithDamage(8);
        WithCostUpgradeBy(-1);
        this.WithTip<GoopPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<DouseInSlimePower>(ctx, this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
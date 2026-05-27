using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class SwordThrow : ChampCardModel
{
    public SwordThrow() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(9, 4);
        this.WithRepeat(2);
        this.WithPower<EntangledNextTurnPower>(1, false);
        WithBerserkerTip();
    }


    protected override bool ShouldGlowRedInternal => !Owner.ShouldBerserkerComboTrigger();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(DynamicVars.Repeat.IntValue).Execute(ctx);
        if (Owner.ShouldBerserkerComboTrigger()) return;
        await CommonActions.ApplySelf<EntangledNextTurnPower>(ctx, this);
    }
}
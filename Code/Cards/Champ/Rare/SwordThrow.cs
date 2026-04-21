using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Champ;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class SwordThrow : ChampCardModel
{
    public SwordThrow() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(9, 4);
        WithVars(new RepeatVar(2));
        WithPower<EntangledNextTurnPower>(1);
        WithTip(typeof(EntangledPower));
    }

    protected override bool ShouldGlowRedInternal => !Owner.ShouldBerserkerComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(DynamicVars.Repeat.IntValue).Execute(ctx);
        if (Owner.ShouldBerserkerComboTrigger()) return;
        await CommonActions.ApplySelf<EntangledNextTurnPower>(this);
    }
}
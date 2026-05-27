using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class ShieldThrow : ChampCardModel
{
    public ShieldThrow() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(0, BlockDamage);
        WithCostUpgradeBy(-1);
        WithDefensiveTip();
        WithTip(StaticHoverTip.Block);
        this.WithPower<NoBlockNextTurnPower>(1, false);
    }


    protected override bool ShouldGlowRedInternal => !Owner.ShouldDefensiveComboTrigger();

    private static decimal BlockDamage(CardModel card, Creature? creature)
    {
        return card.Owner.Creature.Block;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(2).Execute(ctx);
        if (Owner.ShouldDefensiveComboTrigger()) return;
        await CommonActions.ApplySelf<NoBlockNextTurnPower>(ctx, this);
    }
}
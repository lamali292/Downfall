using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class ItMattersNot : CollectorCardModel
{
    public ItMattersNot() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(13, 4);
        WithTip(typeof(VulnerablePower));
        WithTip(typeof(WeakPower));
        WithVar("ItMattersNot", 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardBlock(this, cardPlay);
        var a = CombatState.Enemies.Where(e => e.HasPower<WeakPower>());
        await PowerCmd.Apply<WeakPower>(ctx, a, 1, Owner.Creature, this);
        var b = CombatState.Enemies.Where(e => e.HasPower<VulnerablePower>());
        await PowerCmd.Apply<VulnerablePower>(ctx, b, 1, Owner.Creature, this);
    }
}
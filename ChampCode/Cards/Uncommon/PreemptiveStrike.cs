using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class PreemptiveStrike : ChampCardModel
{
    public PreemptiveStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithCalculatedDamage(0, CalcDamage);
        WithTags(CardTag.Strike);
        WithDefensiveTip();
        WithTip(typeof(CounterPower));
        WithCostUpgradeBy(-1);
    }

    protected override bool ShouldGlowRedInternal => !Owner.ShouldDefensiveComboTrigger();

    private static decimal CalcDamage(CardModel arg1, Creature? arg2)
    {
        return arg1.Owner.Creature.GetPowerAmount<CounterPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (Owner.ShouldDefensiveComboTrigger()) return;
        var a = -Owner.Creature.GetPowerAmount<CounterPower>() / 2;
        if (a >= 0) return;
        await CommonActions.ApplySelf<CounterPower>(ctx, this, a);
    }
}
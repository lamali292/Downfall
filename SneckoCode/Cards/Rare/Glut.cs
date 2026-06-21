using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Glut : SneckoCardModel, IHasOverflowEffect
{

    public Glut() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithOverflow();
        WithDamage(12, 4);
        WithCalculatedVar("OverflowRepeat", 0, Calc);
        WithVar(new DamageVar("OverflowDamage", 2, ValueProp.Move).WithUpgrade(1));
    }

    private static decimal Calc(CardModel card, Creature? _)
        => card.Owner.GetHand().Count(e => e != card);
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var damage = (DamageVar)DynamicVars["OverflowDamage"];
        var hits = (int)((CalculatedVar)DynamicVars["OverflowRepeat"]).Calculate(null);
        if (hits == 0) return;
        await DamageCmd.Attack(damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(ctx);
    }
}
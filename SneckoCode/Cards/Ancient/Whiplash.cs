using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Ancient;

[Pool(typeof(SneckoCardPool))]
public class Whiplash : SneckoCardModel, IHasOverflowEffect
{
    public Whiplash() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
    {
        this.WithOverflow();
        WithDamage(12, 4);
        this.WithTip<WeakPower>();
        this.WithTip<VulnerablePower>();
        WithCalculatedVar("PowerVar", 1, Calc, 1);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return card.EnergyCost.GetResolved();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var x = ((CalculatedVar)DynamicVars["PowerVar"]).Calculate(null);
        await PowerCmd.Apply<WeakPower>(ctx, CombatState.HittableEnemies, x, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(ctx, CombatState.HittableEnemies, x, Owner.Creature, this);
    }
}
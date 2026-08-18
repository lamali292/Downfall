using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.Extensions;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Glut : SneckoCardModel
{
    public Glut() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithOverflow();
        WithDamage(12, 4);
        WithCalculatedVar("OverflowRepeat", 0, Calc);
        WithVar(new DamageVar("OverflowDamage", 2, DamageProps.card).WithUpgrade(1));
    }
    
    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e != card);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var context = await AttackContextCompatibility.CreateContextAsync(CombatState!, ctx, cardPlay);
        try
        {
            context.AddHit(await DownfallCreatureCmd.Damage(
                ctx, cardPlay.Target, DynamicVars.Damage.BaseValue,
                DamageProps.card, this, cardPlay));
            if (SneckoCmd.OverflowActive(this))
            {
                var dmg  = (DamageVar)DynamicVars["OverflowDamage"];
                var hits = (int)((CalculatedVar)DynamicVars["OverflowRepeat"]).Calculate(null);
                for (var i = 0; i < hits; i++)
                {
                    var targets = CombatState!.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList();
                    if (targets.Count == 0) break; 
                    context.AddHit(await DownfallCreatureCmd.Damage(
                        ctx, targets, dmg.BaseValue,
                        DamageProps.card, Owner.Creature, this, cardPlay));
                }
                await SneckoHook.AfterOverflowEffect(Owner, cardPlay, this);
            }
        }
        finally
        {
            await context.DisposeAsync();
        }
    }
}
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class ThrowingCardsPower : SneckoPowerModel
{
    public ThrowingCardsPower()
    {
        WithCards(1);
        WithDamage(0);
        WithTip(DownfallTip.Offclass);
    }

    protected override int? SecondAmount => DynamicVars.Damage.IntValue;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public CardPlay? CardPlay { get; set; }
    
    public void SetDamage(decimal damageBaseValue)
    {
        DynamicVars.Damage.BaseValue = damageBaseValue;
        this.InvokeSecondAmountChanged();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CardPlay == cardPlay)
        {
            CardPlay = null;
            return;
        }
        if (cardPlay.Card.Owner.Creature != Owner) return;
        var card = cardPlay.Card;
        if (!DownfallCmd.IsOffclass(card)) return;
        var a = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        Flash();
        await PowerCmd.Decrement(this);
        if (a != null)
            await CompatibilityCreatureCmd.Damage(ctx, a, DynamicVars.Damage.BaseValue, DamageProps.nonCardUnpowered,
                Owner, null, null);
        await MyCommonActions.Draw(this, ctx);
    }


    
}
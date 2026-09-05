using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class BlunderGuardPower : SneckoPowerModel
{
    public BlunderGuardPower()
    {
        WithEnergy(3);
        WithPower<StrengthPower>(0);
        WithTip(StaticHoverTip.Block);
    }

    private int StrengthAmount => DynamicVars.Power<StrengthPower>().IntValue;

    protected override int? SecondAmount => StrengthAmount;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Resources.EnergySpent < DynamicVars.Energy.BaseValue ||
            cardPlay.Card.Owner.Creature != Owner) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, StrengthAmount, Owner, null);
    }

    public void IncrementStrength(decimal value)
    {
        AssertMutable();
        DynamicVars.Power<StrengthPower>().BaseValue += value;
        this.InvokeSilentDisplayAmountChanged();
    }
}
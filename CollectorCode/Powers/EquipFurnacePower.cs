using BaseLib.Cards.Variables;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class EquipFurnacePower : CollectorPowerModel, IModifyDamageAdditive
{
    public EquipFurnacePower()
    {
        WithTip<Ember>();
        WithTip<Burn>();
        WithTip<Soot>();
        WithVars(MakeCalculatedDamage("Damage", 0, Calc).ToArray());
    }

    private static decimal Calc(PowerModel arg1, Creature? arg2)
    {
        return arg1.Amount * arg1.Owner.Player?.ExhaustPile.Count(e => e is Burn or Ember or Soot) ?? 0;
    }

    public override int DisplayAmount => (int) Calc(this, null);

    public override Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card is not (Burn or Ember or Soot) || card.Owner.Creature != Owner) return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    
    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        var torchhead =  Owner.Player?.Torchhead;
        if (torchhead == null || torchhead != dealer) return 0;
        return ((CustomCalculatedDamageVar)DynamicVars["Damage"]).CalculateCustom(target);
    }
}
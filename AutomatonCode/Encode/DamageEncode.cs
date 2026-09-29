using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class DamageEncode : ValueEncode
{
    public override string Id => "DAMAGE_ENCODE";
    public override int Order => 2;

    public override TargetType Target => TargetType.AnyEnemy;
    public override CardType Type => CardType.Attack;
    public override DynamicVar FunctionDynamicVar => new DamageVar(0, DamageProps.card);

    protected override decimal EnchantedBase(CardModel sourceCard)
    {
        var v = (DamageVar)DynamicVar(sourceCard);
        var e = sourceCard.Enchantment;
        if (e == null) return v.BaseValue;
        var val = v.BaseValue + e.EnchantDamageAdditive(v.BaseValue, v.Props);
        return val * e.EnchantDamageMultiplicative(val, v.Props);
    }

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        if (target == null) return Task.CompletedTask;
        if (model is CardModel card)
            return DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
                .FromCardCompatibility(card, cardPlay)
                .Targeting(target)
                .Execute(ctx);
        return CompatibilityCreatureCmd.Damage(ctx, target, model.DynamicVars.Damage.BaseValue,
            DamageProps.nonCardUnpowered,
            model.Creature, null, null);
    }
}

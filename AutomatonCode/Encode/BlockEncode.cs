using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class BlockEncode : Encodable
{
    public override string Id => "BLOCK_ENCODE";
    public override int Order => 1;
    public override bool GainsBlock => true;

    public override TargetType Target => TargetType.Self;
    public override CardType Type => CardType.Skill;

    public override DynamicVar FunctionDynamicVar => new BlockVar(0, BlockProps.card);

    protected override decimal EnchantedBase(CardModel sourceCard)
    {
        var v = (BlockVar)DynamicVar(sourceCard);
        var e = sourceCard.Enchantment;
        if (e == null) return v.BaseValue;
        var val = v.BaseValue + e.EnchantBlockAdditive(v.BaseValue);
        return val * e.EnchantBlockMultiplicative(val);
    }

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        return CreatureCmd.GainBlock(model.Creature, model.DynamicVars.Block.BaseValue,
            model is CardModel ? BlockProps.card : BlockProps.nonCardUnpowered, cardPlay);
    }
}

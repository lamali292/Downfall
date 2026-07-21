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
    public override TargetType Target => TargetType.Self;
    public override CardType Type => CardType.Skill;

    public override DynamicVar FunctionDynamicVar => new BlockVar(0, ValueProp.Move);

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        return CreatureCmd.GainBlock(model.GetCreature(), model.GetDynamicVars().Block.BaseValue,
            model is CardModel ? ValueProp.Move : ValueProp.Unpowered, cardPlay);
    }

    public override DynamicVar DynamicVar(AbstractModel model)
    {
        return model.GetDynamicVars().Block;
    }
}
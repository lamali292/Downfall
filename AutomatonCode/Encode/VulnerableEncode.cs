using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Encode;

public class VulnerableEncode : Encodable
{
    public override TargetType Target => TargetType.AnyEnemy;
    public override CardType Type => CardType.Skill;
    public override DynamicVar FunctionDynamicVar => new PowerVar<VulnerablePower>(0);

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        if (target == null) return Task.CompletedTask;
        return PowerCmd.Apply<VulnerablePower>(ctx, target,
            model.GetDynamicVars().Vulnerable.BaseValue, model.GetCreature(), model as CardModel);
    }

    public override IEnumerable<IHoverTip> HoverTips(AbstractModel model)
    {
        return [HoverTipFactory.FromPower<VulnerablePower>()];
    }

    public override DynamicVar DynamicVar(AbstractModel model)
    {
        return model.GetDynamicVars().Vulnerable;
    }
}
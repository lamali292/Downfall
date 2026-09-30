using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Encode;

public class PoisonEncode : ValueEncode
{
    public override string Id => "POISON_ENCODE";
    public override int Order => 6;

    public override TargetType Target => TargetType.AnyEnemy;
    public override CardType Type => CardType.Skill;
    public override DynamicVar FunctionDynamicVar => new PowerVar<PoisonPower>(0);

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        if (target == null) return Task.CompletedTask;
        return PowerCmd.Apply<PoisonPower>(ctx, target,
            model.DynamicVars.Poison.BaseValue, model.Creature, model as CardModel);
    }

    public override IEnumerable<IHoverTip> HoverTips(AbstractModel model)
    {
        return [HoverTipFactory.FromPower<PoisonPower>()];
    }
}

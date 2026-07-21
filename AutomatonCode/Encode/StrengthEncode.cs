using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Encode;

public class StrengthEncode : Encodable
{
    public override TargetType Target => TargetType.Self;
    public override CardType Type => CardType.Skill;
    public override DynamicVar FunctionDynamicVar => new PowerVar<StrengthPower>(0);

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        return PowerCmd.Apply<StrengthPower>(ctx, model.GetCreature(),
            model.GetDynamicVars().Strength.BaseValue, model.GetCreature(), model as CardModel);
    }

    public override IEnumerable<IHoverTip> HoverTips(AbstractModel model)
    {
        return [HoverTipFactory.FromPower<StrengthPower>()];
    }

    public override DynamicVar DynamicVar(AbstractModel model)
    {
        return model.GetDynamicVars().Strength;
    }
}
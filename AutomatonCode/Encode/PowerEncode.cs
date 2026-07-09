using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

public class PowerEncode : Encodable
{
    public override TargetType Target => TargetType.Self;
    public override CardType Type =>  CardType.Power;
    public override async Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        if (model is not FunctionCard functionCard) return;
        var fullReleasePower = await CommonActions.ApplySelf<FullReleasePower>(ctx, functionCard);
        fullReleasePower?.SetDynamicalVars(functionCard.DynamicVars);
    }

    public override DynamicVar DynamicVar(AbstractModel model) => model.GetDynamicVars().Power<FullReleasePower>();
    public override DynamicVar FunctionDynamicVar => new PowerVar<FullReleasePower>(0);
}
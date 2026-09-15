using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Compile;

public class ThornsCompile : Compilable
{
    public override DynamicVar FunctionDynamicVar => new("CompileThorns", 0);

    public override Task OnCompile(CardModel card, PlayerChoiceContext ctx)
    {
        return CommonActions.ApplySelf<ThornsPower>(ctx, card);
    }

    public override IEnumerable<IHoverTip> HoverTips(CardModel card)
    {
        return [HoverTipFactory.FromPower<ThornsPower>()];
    }

    protected override decimal GetSourceValue(CardModel card)
    {
        return card.DynamicVars.Power<ThornsPower>().BaseValue;
    }
}

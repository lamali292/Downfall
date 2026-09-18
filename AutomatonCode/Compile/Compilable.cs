using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

public abstract class Compilable
{
    public static readonly IEnumerable<Compilable> All =
    [
        new StrengthCompile(),
        new ThornsCompile(),
        new DazedToDrawCompile(),
        new BurnToDrawCompile(),
        new ErrorToStashCompile(),
        new InfiniteLoopCompile()
    ];

    protected LocString Description => new("encode", GetType().GetPrefix() + StringHelper.Slugify(GetType().Name) + ".compile");

    public abstract DynamicVar FunctionDynamicVar { get; }
    public virtual IEnumerable<DynamicVar> FunctionDynamicVars => [FunctionDynamicVar];

    public abstract Task OnCompile(CardModel card, PlayerChoiceContext ctx);
    public virtual bool MergesOnFunction => true;
    public virtual IEnumerable<IHoverTip> HoverTips(CardModel card) => [];

    public virtual void ApplyCompile(FunctionCard fn, CardModel source)
    {
        fn.DynamicVars[FunctionDynamicVar.Name].BaseValue += GetSourceValue(source);
    }

    protected abstract decimal GetSourceValue(CardModel card);
    
    
    public virtual LocString GetDescription(CardModel card, bool onCard)
    {
        var description = Description;
        DynamicVar dynVar;
        if (card is FunctionCard fn)
        {
            dynVar = fn.DynamicVars[FunctionDynamicVar.Name];
        }
        else
        {
            dynVar = FunctionDynamicVar;
            dynVar.BaseValue = GetSourceValue(card);
        }
        description.Add("OnCard", onCard);
        description.Add(dynVar);
        return description;
    }
}

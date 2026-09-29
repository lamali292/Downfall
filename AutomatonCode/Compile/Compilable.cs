using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Extensions;
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

    /// <summary>
    ///     The real, card-owned DynamicVar backing <see cref="GetSourceValue" />, when one exists
    ///     (e.g. a card's own Strength power var, upgradeable via <c>WithPower(base, upgrade)</c>).
    ///     <see cref="GetDescription" /> copies its "just upgraded" state onto the throwaway compile
    ///     var so the Library's Normal/UG toggle colors the number the same way it already does for
    ///     Encode text and on-card text - a fresh <see cref="FunctionDynamicVar" /> with only
    ///     <see cref="GetSourceValue" />'s plain scalar copied in never carries that state, so
    ///     {CompileX:diff()} never highlights regardless of which version is being previewed.
    ///     Null (the default) when the value never changes with upgrade, e.g. Error To Stash's fixed 1.
    /// </summary>
    protected virtual DynamicVar? GetSourceDynamicVar(CardModel card) => null;

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
            var source = GetSourceDynamicVar(card);
            dynVar.BaseValue = source?.BaseValue ?? GetSourceValue(card);
            if (source is { WasJustUpgraded: true })
                dynVar.UpgradeValueBy(0);
        }
        description.Add("OnCard", onCard);
        description.Add(dynVar);
        return description;
    }
}

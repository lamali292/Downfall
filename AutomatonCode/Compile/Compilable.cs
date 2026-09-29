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
    /// <summary>Loc key part in <c>encode.json</c>: <c>&lt;MOD PREFIX&gt;&lt;Id&gt;.compile</c>. Explicit so renaming the class cannot break loc.</summary>
    public abstract string Id { get; }

    /// <summary>Position in the Function's Compile list (ascending).</summary>
    public abstract int Order { get; }

    protected LocString Description => new("encode", GetType().GetPrefix() + Id + ".compile");

    /// <summary>The Function's var for this effect (a fresh instance each call; the name is what the loc text references).</summary>
    public abstract DynamicVar FunctionDynamicVar { get; }
    public virtual IEnumerable<DynamicVar> FunctionDynamicVars => [FunctionDynamicVar];

    public abstract Task OnCompile(CardModel card, PlayerChoiceContext ctx);
    public virtual bool MergesOnFunction => true;
    public virtual IEnumerable<IHoverTip> HoverTips(CardModel card) => [];

    public virtual void ApplyCompile(FunctionCard fn, CardModel source)
    {
        fn.DynamicVars[FunctionDynamicVar.Name].BaseValue += SourceVar(source).BaseValue;
    }

    /// <summary>
    ///     The single source of this effect's value on <paramref name="card" />: the card's own DynamicVar when
    ///     it has one (e.g. its Strength power var, upgradeable via <c>WithPower(base, upgrade)</c>), or a
    ///     fresh constant var otherwise (Error To Stash's fixed 1). Both the merged Function value and the
    ///     description read it, so the Library's Normal/UG toggle colors the number via the var's
    ///     <c>WasJustUpgraded</c> state.
    /// </summary>
    protected abstract DynamicVar SourceVar(CardModel card);

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
            var source = SourceVar(card);
            dynVar.BaseValue = source.BaseValue;
            if (source.WasJustUpgraded)
                dynVar.UpgradeValueBy(0);
        }
        description.Add("OnCard", onCard);
        description.Add(dynVar);
        return description;
    }
}

using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

/// <summary>
///     A Compile effect that only edits the Function while it is assembled (Retain, a fixed cost): no value is
///     merged and nothing fires when the Function is created. Each source card lists its own line, formatted
///     with that card's vars (<c>&lt;Id&gt;.compile</c> in <c>encode.json</c>), so the text is already visible
///     on the card before the Function exists.
/// </summary>
public abstract class FunctionModifierCompile : Compilable
{
    public sealed override bool MergesOnFunction => false;

    public override DynamicVar FunctionDynamicVar =>
        throw new NotSupportedException($"{GetType().Name} has no Function var.");

    public override IEnumerable<DynamicVar> FunctionDynamicVars => [];

    protected override DynamicVar SourceVar(CardModel card)
    {
        throw new NotSupportedException($"{GetType().Name} has no value.");
    }

    public override Task OnCompile(CardModel card, PlayerChoiceContext ctx)
    {
        return Task.CompletedTask;
    }

    public abstract override void ApplyCompile(FunctionCard fn, CardModel source);

    public override LocString GetDescription(CardModel card, bool onCard)
    {
        var description = Description;
        card.DynamicVars.AddTo(description);
        return description;
    }
}

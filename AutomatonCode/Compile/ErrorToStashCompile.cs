using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

public class ErrorToStashCompile : Compilable
{
    public override string Id => "ERROR_TO_STASH_COMPILE";
    public override DynamicVar FunctionDynamicVar => new("CompileErrors", 0);

    public override Task OnCompile(CardModel card, PlayerChoiceContext ctx)
    {
        return StashCmd.Stash<Error>(ctx, card.Owner);
    }

    protected override DynamicVar SourceVar(CardModel card) => new("CompileErrors", 1);
}

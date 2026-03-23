using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.CardModels;

public interface ICompilableError
{
    Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay, CompileContext compileContext,
        bool forGameplay)
    {
        return Task.CompletedTask;
    }
    
    LocString? CompileErrorLocString => this is CardModel card ? BuildErrorLocString(card) : null;

    
    static LocString? BuildErrorLocString(CardModel card)
    {
        var key = card.Id.Entry + ".error";
        if (!LocString.Exists("encode", key)) return null;
        var loc = new LocString("encode", key);
        card.DynamicVars.AddTo(loc);
        return loc;
    }
}
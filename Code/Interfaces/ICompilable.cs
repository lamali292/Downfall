using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Interfaces;

public interface ICompilable
{
    LocString? CompileLocString => this is CardModel card ? BuildCompileLocString(card) : null;

    Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay, CompileContext compileContext,
        bool forGameplay)
    {
        return Task.CompletedTask;
    }

    static LocString? BuildCompileLocString(CardModel card)
    {
        var key = card.Id.Entry + ".compile";
        if (!LocString.Exists("encode", key)) return null;
        var loc = new LocString("encode", key);
        card.DynamicVars.AddTo(loc);
        return loc;
    }
}
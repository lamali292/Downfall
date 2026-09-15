using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

public abstract class CardToPileCompile<T>(PileType pileType, string varName) : Compilable where T : CardModel
{
    public override DynamicVar FunctionDynamicVar => new(varName, 0);

    public override Task OnCompile(CardModel card, PlayerChoiceContext ctx)
    {
        return DownfallCardCmd.GiveCards<T>(card.Owner, pileType, card.DynamicVars.Cards.BaseValue);
    }

    protected override decimal GetSourceValue(CardModel card)
    {
        return card.DynamicVars.Cards.BaseValue;
    }
}

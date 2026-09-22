using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Compile;

public class InfiniteLoopCompile : Compilable
{
    public override DynamicVar FunctionDynamicVar => new("CompileLoopCount", 0);
    public override bool MergesOnFunction => false;

    public override async Task OnCompile(CardModel card, PlayerChoiceContext ctx)
    {
        var copy = card.CreateClone();
        copy.EnergyCost.AfterCardPlayedCleanup();
        copy.EnergyCost.EndOfTurnCleanup();
        copy.DynamicVars.Damage.UpgradeValueBy(card.DynamicVars["Increase"].BaseValue);
        copy.DynamicVars.FinalizeUpgrade();
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, card.Owner);
    }

    protected override decimal GetSourceValue(CardModel card) => card.DynamicVars["Increase"].BaseValue;
}

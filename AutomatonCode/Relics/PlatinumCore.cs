using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class PlatinumCore : AutomatonRelicModel, IModifyCompiledFunction, IForceEncodesCard
{
    public PlatinumCore() : base(RelicRarity.Starter)
    {
        WithTip<StrikeAutomaton>();
        WithTip<DefendAutomaton>();
        WithTip(AutomatonTip.Encode);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (!ForceEncodes(card)) return;
        await AutomatonCmd.EncodeCard(card, ctx);
        Flash();
    }

    public bool ForceEncodes(CardModel card)
    {
        return Owner == card.Owner && card.IsBasicStrikeOrDefend;
    }

    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (function.SourceCards.Count(e => e.Rarity == CardRarity.Basic) < 2) return false;
        function.EnergyCost.SetUntilPlayed(0);
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        Flash();
        return Task.CompletedTask;
    }
}
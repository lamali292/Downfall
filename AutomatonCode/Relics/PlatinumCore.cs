using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class PlatinumCore : AutomatonRelicModel, IModifyCompiledFunction
{
    public PlatinumCore() : base(RelicRarity.Starter)
    {
        WithCardTip<CoreStrike>();
        WithCardTip<CoreDefend>();
        WithTip(AutomatonKeyword.Encode);
    }

       
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        Flash();
        await Cmd.Wait(0.2f);
        await AutomatonCmd.EncodeCard<CoreStrike>(Owner, ctx);
        await AutomatonCmd.EncodeCard<CoreDefend>(Owner, ctx);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (Owner != card.Owner || !card.IsBasicStrikeOrDefend || card.IsDupe) return;
        await CardCmdCompatibility.Exhaust(ctx, card);
        if (card.Tags.Contains(CardTag.Strike))
        {
            await AutomatonCmd.EncodeCard<CoreStrike>(Owner, ctx);
        }
        else if (card.Tags.Contains(CardTag.Defend))
        {
            await AutomatonCmd.EncodeCard<CoreDefend>(Owner, ctx);
        }
        Flash();
    }
    
    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (function.SourceCards.Count(e => e.Tags.Contains(AutomatonTag.Core)) < 2) return false;
        function.EnergyCost.SetUntilPlayed(0);
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        Flash();
        return Task.CompletedTask;
    }
}
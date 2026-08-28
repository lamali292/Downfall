using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class PlatinumCore : AutomatonRelicModel
{
    public PlatinumCore() : base(RelicRarity.Starter)
    {
        WithTip<StrikeAutomaton>();
        WithTip<DefendAutomaton>();
        WithTip(AutomatonTip.Encode);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner) return;
        if (player.PlayerCombatState is { TurnNumber: 1 })
        {
            await AutomatonCmd.EncodeCard<DefendAutomaton>(Owner, ctx);
            await AutomatonCmd.EncodeCard<StrikeAutomaton>(Owner, ctx);
        }

        var card = AutomatonCmd.GetEncodableCards(player, 1).FirstOrDefault();
        if (card == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        Flash();
    }
}
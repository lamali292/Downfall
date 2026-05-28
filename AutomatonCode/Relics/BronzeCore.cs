using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class BronzeCore : AutomatonRelicModel
{
    public BronzeCore() : base(RelicRarity.Starter)
    {
        this.WithTip<StrikeAutomaton>();
        this.WithTip<DefendAutomaton>();
        WithTip(AutomatonTip.Encode);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<PlatinumCore>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        Flash();
        var card1 = player.Creature.CombatState!.CreateCard(ModelDb.Card<StrikeAutomaton>(), player);
        var card2 = player.Creature.CombatState!.CreateCard(ModelDb.Card<DefendAutomaton>(), player);
        await AutomatonCmd.EncodeCard(card1, ctx);
        await AutomatonCmd.EncodeCard(card2, ctx);
    }
}
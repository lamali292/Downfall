using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Vfx;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
        WithTip<StrikeAutomaton>();
        WithTip<DefendAutomaton>();
        WithTip(AutomatonTip.Encode);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<PlatinumCore>();
    }


    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        Flash();
        await AutomatonCmd.EncodeCard<DefendAutomaton>(Owner, ctx);
        await AutomatonCmd.EncodeCard<StrikeAutomaton>(Owner, ctx);
    }
    
}
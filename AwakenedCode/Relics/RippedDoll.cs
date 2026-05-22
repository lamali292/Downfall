using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class RippedDoll : AwakenedRelicModel
{
    public RippedDoll() : base(RelicRarity.Starter)
    {
        WithTip(AwakenedTip.Conjure);
    }

    public override bool ShowCounter =>
        CombatManager.Instance.IsInProgress && Owner.Creature.CombatState?.RoundNumber < 3;

    public override int DisplayAmount
    {
        get
        {
            var combatState = Owner?.Creature?.CombatState;
            if (combatState == null) return 0;
            if (combatState.RoundNumber > 2) return 0;
            return 2 - combatState.RoundNumber;
        }
    }

    public override Task BeforeCombatStart()
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 2) return;
        Flash();
        await AwakenedCmd.Conjure(Owner, combatState);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<ShreddedDoll>();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}
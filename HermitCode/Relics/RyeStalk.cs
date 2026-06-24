using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Relics;

/// <summary>
/// At the start of turn 4, gain 1 Rugged.
/// </summary>
public sealed class RyeStalk : HermitRelicModel
{
    public RyeStalk() : base(RelicRarity.Rare)
    {
        WithPower<RuggedPower>(1);
        WithVar("Turn", 4);
    }

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState?.TurnNumber != DynamicVars["Turn"].IntValue) return;
        await MyCommonActions.ApplySelf<RuggedPower>(ctx, this);
    }
}
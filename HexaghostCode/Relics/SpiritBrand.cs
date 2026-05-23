using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class SpiritBrand() : HexaghostRelicModel(RelicRarity.Starter), IAfterGhostflameIgnited
{
    private bool UsedThisTurn { get; set; }

    public async Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player != Owner || UsedThisTurn) return;
        UsedThisTurn = true;
        Flash();
        Status = RelicStatus.Normal;
        await CreatureCmd.GainBlock(Owner.Creature, 4, ValueProp.Move | ValueProp.Unpowered, null, true);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<MarkOfTheEther>();
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side) return Task.CompletedTask;
        Status = RelicStatus.Active;
        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}
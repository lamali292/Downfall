using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class ViperSkull : SneckoRelicModel
{
    public ViperSkull() : base(RelicRarity.Rare)
    {
        WithPower<VenomPower>(3);
    }
    
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {

        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState == null || Owner.PlayerCombatState.TurnNumber > 1)
            return;
        Flash();
        await PowerCmd.Apply<VulnerablePower>(choiceContext, combatState.HittableEnemies, DynamicVars.Power<VenomPower>().BaseValue, Owner.Creature,null);
    }
}
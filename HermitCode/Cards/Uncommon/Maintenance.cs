using BaseLib.Extensions;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Gain 1 Dexterity. This costs 1 more this combat.
///     Upgrade: Gain 2 Dexterity.
/// </summary>
public sealed class Maintenance : HermitCardModel
{
    public Maintenance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<MaintenanceStrikePower>(3, 1);
        WithPower<DexterityPower>(1, 1);
        WithTip(HermitKeywords.Strike);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<MaintenanceStrikePower>(ctx, Owner.Creature,
            DynamicVars.Power<MaintenanceStrikePower>().IntValue, Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(ctx, Owner.Creature, DynamicVars.Power<DexterityPower>().IntValue,
            Owner.Creature, this);
        EnergyCost.UpgradeBy(1);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithPower<MaintenanceStrikePower>(3, 1), WithPower<DexterityPower>(1, 1)
 */
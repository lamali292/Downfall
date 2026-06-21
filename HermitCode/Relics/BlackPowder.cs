using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you trigger a Dead On effect, deal 2 damage to ALL enemies.
/// </summary>
public sealed class BlackPowder : HermitRelicModel, IAfterDeadOnTrigger
{
    public BlackPowder() : base(RelicRarity.Common)
    {
        WithVars(new DamageVar(2, ValueProp.Unpowered));
        WithTip(HermitKeywords.DeadOn);
    }

    public async Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(ctx, Owner.Creature.CombatState!.HittableEnemies, DynamicVars.Damage, Owner.Creature);
    }
}
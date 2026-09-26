using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

// "Shares a Potency stat" needs no special code - Potency is already owner-scoped (PotencyPower boosts every
// slime the owner controls equally), so multiple Darklings already share it automatically.
public class DarklingSlime : SlimeModel, IAfterCommand
{
    public override SlimeType SlimeType => SlimeType.None;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.nonCardUnpowered)
    ];

    // NOTE: no scene/skin exists for this slime yet - reusing Insulting's "champ" skin as a placeholder.
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("champ"));
        skeleton.SetSlotsToSetupPose();
    }

    public override Task Command(PlayerChoiceContext ctx)
    {
        return DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this)
            .TargetingRandomOpponents(CombatState).Execute(ctx);
    }

    // "Deals 3 damage to a random enemy... whenever another Slime is Commanded."
    public async Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source)
    {
        if (player.Creature != PetOwner || slime == this || slime is DarklingSlime) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this)
            .TargetingRandomOpponents(CombatState).Execute(ctx);
    }
}

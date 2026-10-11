using System.Security.Cryptography;
using System.Text;
using BaseLib.Abstracts;
using Collector.CollectorCode.Intents;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Core;

public class TorchheadMonsterModel : CustomMonsterModel
{
    public override string CustomVisualPath =>
        "res://Collector/scenes/character/torchhead_combat.tscn";

    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;

    public override float DeathAnimLengthOverride => 0.2f;
    public override bool HasHurtSfx => false;
    public override bool HasDeathSfx => false;

    public override bool IsHealthBarVisible => Creature.IsAlive;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        // Torchhead doesn't act through this state machine - the real attack fires from
        // TorchheadPower.AfterSideTurnEnd. This state only exists to carry an intent to display.
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask, new TorchheadAttackIntent());
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Creature) return;
        await CreatureCmd.SetMaxHp(target, Creature.CurrentHp);
        
        TorchheadCmd.RefreshTorchheadScale(target);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (creature == Creature)
            NCombatRoom.Instance?.GetCreatureNode(creature)?.AnimHideIntent();

        return base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
    }
    
    private static readonly HashSet<string> PinkSkinPlayerHashes =
    [
        "054DAB2BA8A437F0752FEB3823FE68D0C14FCDD1681E5E8006EF88AFFEBF7C5C"
    ];

    private static string HashPlayerId(ulong id) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"downfall-skin:{id}")));

    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        var skinName = GetOwnerPlatformId() is { } id && PinkSkinPlayerHashes.Contains(HashPlayerId(id))
            ? "pink"
            : "normal";
        skeleton.SetSkin(skeleton.GetData().FindSkin(skinName));
        skeleton.SetSlotsToSetupPose();
    }

    /// <summary>
    ///     Steam64 ID of the pet's owner. In multiplayer <c>Player.NetId</c> already is the platform ID; in
    ///     singleplayer it is the placeholder <c>1</c>, and the only player is the local one.
    /// </summary>
    private ulong? GetOwnerPlatformId()
    {
        var netId = Creature.PetOwner?.NetId;
        return netId == NetSingleplayerGameService.defaultNetId ? PlatformUtil.GetLocalPlayerId(PlatformUtil.PrimaryPlatform) : netId;
    }

    public override CreatureAnimator SetupCustomAnimationStates(MegaSprite controller)
    {
        var idleState = new AnimState("idle_loop", true);
        var castState = new AnimState("cast");
        var attackState = new AnimState("attack");
        var hurtState = new AnimState("hurt");
        var dieState = new AnimState("die");
        //var deadLoopState = new AnimState("dead_loop", true);
        var reviveState = new AnimState("revive");
        idleState.AddBranch("Hit", hurtState);
        castState.NextState = idleState;
        castState.AddBranch("Hit", hurtState);
        attackState.NextState = idleState;
        attackState.AddBranch("Hit", hurtState);
        hurtState.NextState = idleState;
        hurtState.AddBranch("Hit", hurtState);
        //dieState.NextState = deadLoopState;
        reviveState.NextState = idleState;
        var animator = new CreatureAnimator(idleState, controller);
        animator.AddAnyState("Attack", attackState);
        animator.AddAnyState("Cast", castState);
        animator.AddAnyState("Dead", dieState);
        animator.AddAnyState("Revive", reviveState);
        return animator;
    }
}
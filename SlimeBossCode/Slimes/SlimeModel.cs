using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public abstract class SlimeModel : CustomMonsterModel
{
    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;
    public abstract SlimeType SlimeType { get; }

    public override string CustomVisualPath =>
        $"combat/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".SlimeScenePath();

    public override bool HasDeathSfx => false;
    public override bool IsHealthBarVisible => Creature.IsAlive;
    public Creature PetOwner => Creature.PetOwner?.Creature ?? throw new ArgumentNullException(nameof(PetOwner));

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var initialState = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        initialState.FollowUpState = initialState;
        return new MonsterMoveStateMachine([initialState], initialState);
    }

    public abstract Task Command(PlayerChoiceContext ctx);
    protected virtual LocString Description => L10NMonsterLookup(Id.Entry + ".description");

    public HoverTip SlimeTip => new(Title, Description);
    
    public virtual IEnumerable<IHoverTip> ExtraTips => [];
}

[Flags]
public enum SlimeType
{
    None = 0,
    Normal = 1,
    Specialist = 2,
    Any = Normal | Specialist
}

[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived))]
internal static class PersonalHivePowerNoPetCardsPatch
{
    static bool Prefix(Creature? dealer, ref Task __result)
    {
        if (dealer?.Monster is not SlimeModel) return true; 
        __result = Task.CompletedTask;
        return false;

    }
}

[HarmonyPatch(typeof(Creature), nameof(Creature.HoverTips), MethodType.Getter)]
internal static class SlimeHoverTipPatch
{
    static void Postfix(Creature __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.Monster is not SlimeModel slime) return;
        __result = __result.Append(slime.SlimeTip);
        __result = __result.Concat(slime.ExtraTips);
    }
}
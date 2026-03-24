using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Downfall.Code.Abstract;

public abstract class DownfallCharacterModel<TCharacter> : CustomCharacterModel
    where TCharacter : DownfallCharacterModel<TCharacter>
{
    public virtual string? CharId => null;
    protected virtual Color EnergyOutlineColor => new(0, 0, 0);
    protected virtual Color EnergyBurstColor => new(1, 1, 1);
    public virtual Color LabOutlineColor => new(1, 1, 1);
    public virtual Color DeckEntryCardColor => new(1, 1, 1);
    public virtual Color CardHsv => new(1, 1, 1);

    private string? _id => CharId!.ToSnakeCase();

    public override string CustomCharacterSelectBg =>
        $"res://Downfall/character/scenes/selection_screen/selection_screen_{_id}.tscn";

    public override string CustomCharacterSelectIconPath =>
        $"res://Downfall/character/selection_screen/char_select/char_select_{_id}.png";

    public override string CustomCharacterSelectLockedIconPath =>
        $"res://Downfall/character/selection_screen/char_select/char_select_{_id}_locked.png";

    public override string CustomIconTexturePath =>
        $"res://Downfall/character/icons/character_icon_{_id}.png";


    public override CustomEnergyCounter? CustomEnergyCounter =>
        new CustomEnergyCounter(EnergyCounterPaths, EnergyOutlineColor, EnergyBurstColor);

    public override string CustomEnergyCounterPath => "res://Downfall/scenes/energy_counter_empty.tscn";

    public override string CustomMapMarkerPath =>
        $"res://Downfall/character/map_marker/map_marker_{_id}.png";

    public override string CustomArmPointingTexturePath =>
        $"res://Downfall/character/arms/multiplayer_hand_{_id}_point.png";

    public override string CustomArmRockTexturePath =>
        $"res://Downfall/character/arms/multiplayer_hand_{_id}_rock.png";

    public override string CustomArmPaperTexturePath =>
        $"res://Downfall/character/arms/multiplayer_hand_{_id}_paper.png";

    public override string CustomArmScissorsTexturePath =>
        $"res://Downfall/character/arms/multiplayer_hand_{_id}_scissors.png";

    public override string CustomCharacterSelectTransitionPath =>
        $"res://Downfall/character/transitions/{_id}_transition_mat.tres";

    public override string CustomVisualPath =>
        $"res://Downfall/character/scenes/combat_scene/{_id}_combat.tscn";

    public override string CustomIconPath => $"res://Downfall/character/scenes/icon/{_id}_icon.tscn";
    public override string CustomTrailPath => $"res://Downfall/character/scenes/card_trail/card_trail_{_id}.tscn";
    public override string CustomRestSiteAnimPath => "res://Downfall/scenes/watcher/watcher_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://Downfall/scenes/watcher/watcher_merchant.tscn";


    public override string CustomAttackSfx => "res://";
    public override string CustomCastSfx => "res://";
    public override string CustomDeathSfx => "res://";

    private string EnergyCounterPaths(int i)
    {
        return $"res://Downfall/character/energy_counters/big/{CharId!.ToSnakeCase()}_orb_layer_" + i + ".png";
    }


    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash", "vfx/vfx_bloody_impact",
            "vfx/vfx_rock_shatter"
        ];
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        GD.Print("[Downfall] GenerateAnimator called");

        var animState = new AnimState("Idle", true);
        var state1 = new AnimState("Idle");
        var state2 = new AnimState("Idle");
        var state3 = new AnimState("Hit");
        var state4 = new AnimState("Idle");
        var state5 = new AnimState("Idle");
        state1.NextState = animState;
        state2.NextState = animState;
        state3.NextState = animState;
        state5.NextState = animState;
        state5.AddBranch("Idle", animState);
        var animator = new CreatureAnimator(animState, controller);
        animator.AddAnyState("Idle", animState);
        animator.AddAnyState("Dead", state4);
        animator.AddAnyState("Hit", state3);
        animator.AddAnyState("Attack", state2);
        animator.AddAnyState("Cast", state1);
        animator.AddAnyState("Relaxed", state5);

        return animator;
    }
}
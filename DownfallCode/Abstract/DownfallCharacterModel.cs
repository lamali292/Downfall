using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallCharacterModel : CustomCharacterModel
{
    protected DownfallCharacterModel()
    {
        DownfallMainFile.Logger.Info($"Creating {GetType().Name}");
    }

    public abstract string ModId { get; }
    public abstract string? CharId { get; }
    public virtual Color LabOutlineColor => new(1, 1, 1);
    public virtual Color DeckEntryCardColor => new(1, 1, 1);
    public abstract float CardColorH { get; }
    public abstract float CardColorS { get; }
    public abstract float CardColorV { get; }

    public override string CustomCharacterSelectBg =>
        $"res://{ModId}/scenes/character/selection_screen.tscn";

    public override string CustomCharacterSelectIconPath =>
        $"res://{ModId}/images/character/char_select.png";

    public override string CustomCharacterSelectLockedIconPath =>
        $"res://{ModId}/images/character/char_select_locked.png";

    public override string CustomIconTexturePath =>
        $"res://{ModId}/images/character/character_icon.png";


    public override RelicIconData CustomYummyCookie => new(
        "yummy.png".BigRelicImagePath(ModId),
        "yummy.tres".TresRelicImagePath(ModId),
        "yummy_outline.tres".TresRelicImagePath(ModId)
    );

    /*
    public override CustomEnergyCounter? CustomEnergyCounter =>
        new CustomEnergyCounter(EnergyCounterPaths, EnergyOutlineColor, EnergyBurstColor);
*/
    public override string CustomEnergyCounterPath
    {
        get
        {
            var path = $"res://{ModId}/scenes/character/energy_counter.tscn";
            return ResourceLoader.Exists(path)
                ? path
                : "res://Downfall/scenes/character/energy_counter_empty.tscn";
        }
    }


    public override string CustomMapMarkerPath =>
        $"res://{ModId}/images/character/map_marker.png";

    public override string CustomArmPointingTexturePath =>
        $"res://{ModId}/images/character/mp_point.png";

    public override string CustomArmRockTexturePath =>
        $"res://{ModId}/images/character/mp_rock.png";

    public override string CustomArmPaperTexturePath =>
        $"res://{ModId}/images/character/mp_paper.png";

    public override string CustomArmScissorsTexturePath =>
        $"res://{ModId}/images/character/mp_scissors.png";

    public override string CustomCharacterSelectTransitionPath =>
        $"res://{ModId}/material/character/transition_mat.tres";

    public override string CustomVisualPath =>
        $"res://{ModId}/scenes/character/combat.tscn";

    public override string CustomIconPath => $"res://{ModId}/scenes/character/char_icon.tscn";

    public override string CustomIconOutlineTexturePath =>
        $"{ModId}/images/character/character_icon_outline.png";

    public override string CustomTrailPath => $"res://{ModId}/scenes/character/card_trail.tscn";
    public override string CustomRestSiteAnimPath => "res://Downfall/scenes/character/error_rest_site.tscn";
    public override string CustomMerchantAnimPath => $"res://{ModId}/scenes/character/merchant.tscn";


    public override string CustomAttackSfx => "event:/sfx/characters/ironclad/ironclad_attack";

    //public override string CustomCastSfx => "res://";
    public override string CustomDeathSfx => "event:/sfx/characters/ironclad/ironclad_die";

    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash", "vfx/vfx_bloody_impact",
            "vfx/vfx_rock_shatter"
        ];
    }

    /// <summary>
    ///     Non-looping animation states (and the trigger that plays them) built for every custom
    ///     character's animator. Override to add character-specific states (see Champ's jump attack) —
    ///     a fresh list/AnimState instances is returned on every access since each GenerateAnimator call
    ///     needs its own AnimState objects.
    ///
    ///     Unrelated to (and deliberately hides, not overrides) the same-named vanilla
    ///     CharacterModel.AnimationStates: this one feeds only SetupCustomAnimationStates (BaseLib's
    ///     GenerateAnimatorPatch prefix), which short-circuits vanilla GenerateAnimator entirely for any
    ///     custom character, so the vanilla property is never reached through this type.
    /// </summary>
    protected new virtual List<(AnimState state, string trigger)> AnimationStates
    {
        get
        {
            var cast = new AnimState("cast");
            return
            [
                (cast, CreatureAnimator.castTrigger),
                (cast, CreatureAnimator.powerUpTrigger),
                (new AnimState("attack"), CreatureAnimator.attackTrigger),
                (new AnimState("hurt"), CreatureAnimator.hitTrigger)
            ];
        }
    }

    /// <summary>
    ///     25% max HP or below, matching the low-health idle/animation variants used by some characters.
    ///     Unrelated to (and deliberately hides) vanilla CharacterModel.IsLowHealth — see <see cref="AnimationStates"/>.
    /// </summary>
    protected new static bool IsLowHealth(Creature creature) => creature.CurrentHp <= creature.MaxHp * 0.25f;
}
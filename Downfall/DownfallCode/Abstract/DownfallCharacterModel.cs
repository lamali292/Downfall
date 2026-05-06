using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallCharacterModel : CustomCharacterModel
{
    public DownfallCharacterModel()
    {
        DownfallMainFile.Logger.Info($"Creating {GetType().Name}");
    }

    public abstract string ModId { get; }
    public abstract string? CharId { get; }
    protected virtual Color EnergyOutlineColor => new(0, 0, 0);
    protected virtual Color EnergyBurstColor => new(1, 1, 1);
    public virtual Color LabOutlineColor => new(1, 1, 1);
    public virtual Color DeckEntryCardColor => new(1, 1, 1);
    public virtual Color CardColor => new(1, 1, 1);

    public override string CustomCharacterSelectBg =>
        $"res://{ModId}/scenes/character/selection_screen.tscn";

    public override string CustomCharacterSelectIconPath =>
        $"res://{ModId}/images/character/char_select.png";

    public override string CustomCharacterSelectLockedIconPath =>
        $"res://{ModId}/images/character/char_select_locked.png";

    public override string CustomIconTexturePath =>
        $"res://{ModId}/images/character/character_icon.png";


    public override CustomEnergyCounter? CustomEnergyCounter =>
        new CustomEnergyCounter(EnergyCounterPaths, EnergyOutlineColor, EnergyBurstColor);

    public override string CustomEnergyCounterPath => "res://Downfall/scenes/character/energy_counter_empty.tscn";

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
    public override string CustomMerchantAnimPath => "res://Downfall/scenes/character/error_merchant.tscn";


    public override string CustomAttackSfx => "event:/sfx/characters/ironclad/ironclad_attack";

    //public override string CustomCastSfx => "res://";
    public override string CustomDeathSfx => "event:/sfx/characters/ironclad/ironclad_die";
    public override string CharacterSelectSfx => "event:/sfx/characters/ironclad/ironclad_select";//"event:/selection/selection_gremlins"; //"event:/sfx/characters/ironclad/ironclad_select";

    private string EnergyCounterPaths(int i)
    {
        return $"res://{ModId}/images/character/orb_layer_{i}.png";
    }


    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash", "vfx/vfx_bloody_impact",
            "vfx/vfx_rock_shatter"
        ];
    }
}
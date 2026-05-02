using Downfall.DownfallCode.Interfaces;
using Godot;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Core;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Guardian.GuardianCode.Powers;

public class BrilliantScalesPower : GuardianPowerModel
{
    public override bool IsInstanced => true;
    private GuardianCardModel? _sourceCard;
    private IReadOnlyList<GemModel> Gems => _sourceCard?.Gems ?? [];
    public event Action? GemsChanged;
    public BrilliantScalesPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTips(power => ((BrilliantScalesPower)power).Gems?
            .SelectMany(gem => gem.HoverTips) ?? []);
        WithVars(new BrilliantScalesDynamicVar());
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (_sourceCard == null) return;
        if (Owner != player.Creature) return;
        foreach (var gem in _sourceCard.Gems)
            await gem.OnPlayWrapper(ctx, null); }

    public void SetCard(GuardianCardModel cardModel)
    {
        _sourceCard = cardModel;
        foreach (var sourceCardGem in _sourceCard.Gems)
        {
            sourceCardGem.Power = this;
        }
        GemsChanged?.Invoke();
    }

    
    private class BrilliantScalesDynamicVar : DynamicVar
    {
        private BrilliantScalesPower? _power;

        public BrilliantScalesDynamicVar() : base("effects", 0)
        {
        }

        public override void SetOwner(AbstractModel model)
        {
            base.SetOwner(model);
            _power = model as BrilliantScalesPower;
        }

        public override string ToString()
        {
            if (_power == null) return "";
            var lines = _power.Gems
                .Select(loc => loc.GetFormattedText())
                .ToList();
            return lines.Count > 0 ? string.Join("\n", lines) : "";
        }
    }
    
    // Patch — subscribe in _Ready, build icons when gems actually arrive
    [HarmonyPatch(typeof(NPower))]
    internal static class BrilliantScalesGemPatch
    {
        [HarmonyPatch("_Ready")]
        [HarmonyPostfix]
        private static void SubscribeToGems(NPower __instance)
        {
            if (__instance._model is not BrilliantScalesPower power) return;

            power.GemsChanged += () => RefreshGemTextures(__instance);

            if (power.Gems.Count > 0)
                RefreshGemTextures(__instance);
        }

        private static void RefreshGemTextures(NPower instance)
        {
            if (!GodotObject.IsInstanceValid(instance)) return;
            if (instance._model is not BrilliantScalesPower power) return;

            var icon = instance.GetNode<TextureRect>("%Icon");

            foreach (var child in icon.GetChildren())
                if (child.Name.ToString().StartsWith("gem_slot_"))
                    child.QueueFree();

            var gems = power.Gems.ToList();
            if (gems.Count == 0) return;

            float[] rotations = gems.Count switch
            {
                1 => [0f],
                2 => [-45f, 135f],
                _ => [0f, 120f, -120f],
            };
            var shaderMaterial = (ShaderMaterial)icon.Material;
            for (var i = 0; i < gems.Count; i++)
            {
                var rect = new TextureRect
                {
                    Name        = $"gem_slot_{i}",
                    Texture     = gems[i].Icon,
                    Material     = shaderMaterial,   
                    OffsetLeft  = 10f,
                    OffsetTop   = -2f,
                    OffsetRight = 30f,
                    OffsetBottom = 18f,
                    PivotOffset = new Vector2(10f, 22f),
                    Rotation    = Mathf.DegToRad(rotations[i]),
                    ExpandMode  = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                };
                icon.AddChild(rect);
            }
        }
    }
    
}


using Downfall.DownfallCode.Interfaces;
using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Guardian.GuardianCode.Powers;

public class BrilliantScalesPower : GuardianPowerModel, ICustomPowerIcon
{
    
    // TODO: Now and at the start of every other turn. not every turn.
    private IGemSocketCard? _sourceCard;

    public BrilliantScalesPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTips(power => ((BrilliantScalesPower)power).Gems?
            .SelectMany(gem => gem.HoverTips) ?? []);
        WithVars(new BrilliantScalesDynamicVar());
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    private IReadOnlyList<GemModel> Gems => _sourceCard?.Gems ?? [];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (_sourceCard == null) return;
        if (Owner != player.Creature) return;
        foreach (var gem in _sourceCard.Gems)
            await gem.OnPlay(ctx, null);
    }

    public void SetCard(IGemSocketCard cardModel)
    {
        _sourceCard = cardModel;
        foreach (var sourceCardGem in _sourceCard.Gems) sourceCardGem.Power = this;
        IconChanged?.Invoke();
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
    

    public void DecorateIcon(TextureRect icon)
    {
        var gems = Gems.ToList();
        if (gems.Count == 0) return;

        float[] rotations = gems.Count switch
        {
            1 => [0f],
            2 => [-45f, 135f],
            _ => [0f, 120f, -120f]
        };
        var shaderMaterial = (ShaderMaterial)icon.Material;

        for (var i = 0; i < gems.Count; i++)
        {
            icon.AddDecoration(new TextureRect
            {
                Texture = gems[i].Icon,
                Material = shaderMaterial,
                OffsetLeft = 10f, OffsetTop = -2f, OffsetRight = 30f, OffsetBottom = 18f,
                PivotOffset = new Vector2(10f, 22f),
                Rotation = Mathf.DegToRad(rotations[i]),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspect
            }, i);
        }
    }

    public event Action? IconChanged;
}
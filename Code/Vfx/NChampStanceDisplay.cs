// NChampStanceDisplay.cs
using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Vfx;

public partial class NChampStanceDisplay : Control
{
    private const string InactiveChargePath = "res://Downfall/images/ui/stance_charge_inactive.png";
    private const int ChargeIconSize = 70;
    private const int Separation = 6;
    private const int IconCount = 3;
    private const int TotalWidth = IconCount * ChargeIconSize + (IconCount - 1) * Separation;
    private const int TotalHeight = ChargeIconSize;
    private const int MarginAboveHead = 20;

    private Player? _trackedPlayer;
    private Control? _bounds;
    private readonly List<TextureRect> _icons = new();

    public static NChampStanceDisplay? Show(Player player)
    {
        var combatRoom = NCombatRoom.Instance;

        var creatureNode = combatRoom?.GetCreatureNode(player.Creature);
        if (creatureNode == null) return null;

        var display = new NChampStanceDisplay
        {
            _trackedPlayer = player,
            _bounds = creatureNode.Visuals.Bounds,
            ZIndex = creatureNode.ZIndex - 1
        };

        combatRoom?.CombatVfxContainer.AddChildSafely(display);
        return display;
    }

    public override void _Ready()
    {
        // Set our own size explicitly — no layout system involved
        Size = new Vector2(TotalWidth, TotalHeight);

        // Create icons at known positions
        for (var i = 0; i < IconCount; i++)
        {
            var icon = new TextureRect
            {
                StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                Size = new Vector2(ChargeIconSize, ChargeIconSize),
                Position = new Vector2(i * (ChargeIconSize + Separation), 0)
            };
            AddChild(icon);
            _icons.Add(icon);
        }

        Reposition();
        Refresh();
    }

    private void Reposition()
    {
        if (_bounds == null) return;

        GlobalPosition = new Vector2(
            _bounds.GlobalPosition.X + _bounds.Size.X / 2f - TotalWidth / 2f,
            _bounds.GlobalPosition.Y - TotalHeight - MarginAboveHead
        );
    }

    public void Refresh()
    {
        if (_trackedPlayer == null || _icons.Count == 0) return;

        var stance = _trackedPlayer.ChampStance();

        if (stance is NoChampStance or null)
        {
            QueueFree();
            return;
        }

        var activePath = stance.ChargeIconPath ?? InactiveChargePath;

        for (var i = 0; i < _icons.Count; i++)
        {
            var isActive = i < stance.Charges;
            _icons[i].Texture = ResourceLoader.Load<Texture2D>(isActive ? activePath : InactiveChargePath);
        }
    }
}
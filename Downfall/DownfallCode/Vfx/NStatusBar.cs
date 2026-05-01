using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.DownfallCode.Vfx;

public partial class NStatusBar : Control
{
    private int _currentCurrent;
    private int _currentMax;
    private MegaLabel _label = null!;
    private NStatusPart[] _parts = null!;

    public Control HpBarContainer { get; private set; } = null!;

    public override void _Ready()
    {
        HpBarContainer = GetNode<Control>("%HpBarContainer");
        var hbox = GetNode<HBoxContainer>("HpBarContainer/HBoxContainer");
        _label = GetNode<MegaLabel>("%Label");
        _parts = hbox.GetChildren().OfType<NStatusPart>().ToArray();

        _label.Visible = false;
        foreach (var part in _parts)
        {
            part.Visible = true;
            part.Modulate = Colors.White;
        }

        _currentMax = 0;
        _currentCurrent = 0;
        SetStatus(0, 0);
    }

    public void SetStatus(int current, int max, Color? color = null)
    {
        Visible = true;
        for (var i = 0; i < _parts.Length; i++)
        {
            var shouldBeVisible = i < max;
            var filled = i < current;
            _parts[i].Visible = shouldBeVisible;
            _parts[i].Show(filled, color);
        }


        _currentCurrent = current;
        _currentMax = max;
    }


    public void UpdateLayoutForCreatureBounds(Control bounds)
    {
        HpBarContainer.GlobalPosition = new Vector2(
            bounds.GlobalPosition.X,
            HpBarContainer.GlobalPosition.Y);
        HpBarContainer.Size = new Vector2(bounds.Size.X, HpBarContainer.Size.Y);
    }
}

public static class StatusBarHelper
{
    private const string NodeName = "ExtraStatusBar";

    public static NStatusBar? Get(Player player)
    {
        return NCombatRoom.Instance?
            .GetCreatureNode(player.Creature)?
            ._stateDisplay
            .GetNodeOrNull<NStatusBar>(NodeName);
    }

    public static void SetStatus(Player player, int current, int max, Color? color)
    {
        Get(player)?.SetStatus(current, max, color);
    }
}
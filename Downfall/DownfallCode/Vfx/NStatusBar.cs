using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.DownfallCode.Vfx;

public partial class NStatusBar : Control
{
    private Control _hpBarContainer = null!;
    private NStatusPart[] _parts = null!;
    private MegaLabel _label = null!;
    private int _currentCurrent;
    private int _currentMax;

    public Control HpBarContainer => _hpBarContainer;

    public override void _Ready()
    {
        _hpBarContainer = GetNode<Control>("%HpBarContainer");
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
        _hpBarContainer.GlobalPosition = new Vector2(
            bounds.GlobalPosition.X,
            _hpBarContainer.GlobalPosition.Y);
        _hpBarContainer.Size = new Vector2(bounds.Size.X, _hpBarContainer.Size.Y);
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
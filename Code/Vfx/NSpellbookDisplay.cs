using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Downfall.Code.Vfx;

public partial class NSpellbookDisplay : Control
{
    private const float IconSize = 64f;
    private const float IconDistance = IconSize + 8f;
    private readonly float[] _bobOffsets = new float[8];
    private readonly float[] _bobSpeeds = [1.1f, 0.9f, 1.05f, 0.95f, 1.0f, 0.85f, 1.15f, 0.98f];

    private readonly List<TextureRect> _iconNodes = [];
    private float _bobTime;

    private Player? _trackedPlayer;

    public static NSpellbookDisplay Create(Player player)
    {
        return new NSpellbookDisplay
        {
            _trackedPlayer = player,
            Position = Vector2.Zero
        };
    }

   public void Refresh()
{
    if (_trackedPlayer == null) return;

    // Clean up old icons
    foreach (var icon in _iconNodes) icon.QueueFree();
    _iconNodes.Clear();

    var spellbook = AwakenedCmd.GetSpellbook(_trackedPlayer);
    if (spellbook == null) return;

    // GROUPING: Group cards by their Model ID (or Type)
    var groupedCards = spellbook.Cards
        .GroupBy(c => c.Id) 
        .ToList();

    for (var i = 0; i < groupedCards.Count; i++)
    {
        var group = groupedCards[i];
        var firstCard = group.First(); // Use the first instance for the icon/tooltips
        var count = group.Count();
        
        if (firstCard is not ISpell spell) continue;

        var iconPath = spell.SpellIconPath;
        if (!ResourceLoader.Exists(iconPath)) continue;

        // Visual logic for the "Next Spell"
        var isNext = firstCard == spellbook.NextSpell || group.Contains(spellbook.NextSpell);
        
        var icon = new TextureRect
        {
            Texture = ResourceLoader.Load<Texture2D>(iconPath),
            StretchMode = TextureRect.StretchModeEnum.KeepAspect,
            CustomMinimumSize = new Vector2(IconSize + (isNext ? 12 : 0), IconSize + (isNext ? 12 : 0)),
            // We use 'i' (the group index) for positioning
            Position = new Vector2(i * IconDistance - (isNext ? 6 : 0), isNext ? -6 : 0),
            MouseFilter = MouseFilterEnum.Stop
        };

        // ADD COUNTER: Only if there is more than one
        if (count > 1)
        {
            var label = new Label
            {
                Text = $"{count}x",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Size = icon.CustomMinimumSize,
                Position = new Vector2(4, 4), // Offset slightly from the corner
            };
            
            // Give the label a shadow or outline so it's readable over the icon
            label.AddThemeColorOverride("font_outline_color", Colors.Black);
            label.AddThemeConstantOverride("outline_size", 4);
            
            icon.AddChild(label);
        }
        
        icon.MouseEntered += () =>
        {
            var tip = HoverTipFactory.FromCard(firstCard);
            NHoverTipSet.CreateAndShow(icon, tip, HoverTipAlignment.Center);
        };
        icon.MouseExited += () => NHoverTipSet.Remove(icon);

        AddChild(icon);
        _iconNodes.Add(icon);
    }
}

public override void _Process(double delta)
{
    if (_trackedPlayer == null || !CombatManager.Instance.IsInProgress) return;

    _bobTime += (float)delta;
    // Update bobbing logic
    for (var i = 0; i < _bobOffsets.Length; i++)
        _bobOffsets[i] = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 4f;

    // Position the grouped icons
    for (var i = 0; i < _iconNodes.Count; i++)
    {
        var isNext = _iconNodes[i].CustomMinimumSize.X > IconSize; // Check if it's the "Next" spell
        _iconNodes[i].Position = new Vector2(
            i * IconDistance - (isNext ? 6 : 0), 
            (i < _bobOffsets.Length ? _bobOffsets[i] : 0f) - (isNext ? 6 : 0)
        );
    }
}
}
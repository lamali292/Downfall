using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Downfall.Code.Cards.Vfx;

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

        foreach (var icon in _iconNodes) icon.QueueFree();
        _iconNodes.Clear();

        var spellbook = AwakenedCmd.GetSpellbook(_trackedPlayer);
        if (spellbook == null) return;

        var cards = spellbook.Cards;
        for (var i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            if (card is not ISpell spell) continue;

            var iconPath = spell.SpellIconPath;
            if (!ResourceLoader.Exists(iconPath)) continue;

            var isNext = card == spellbook.NextSpell;
            
            var icon = new TextureRect
            {
                Texture = ResourceLoader.Load<Texture2D>(iconPath),
                StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                CustomMinimumSize = new Vector2(IconSize + (isNext ? 12 : 0), IconSize + (isNext ? 12 : 0)),
                Position = new Vector2(i * IconDistance - (isNext ? 6 : 0), isNext ? -6 : 0),
                MouseFilter = MouseFilterEnum.Stop
            };
            
            icon.MouseEntered += () =>
            {
                var tip = HoverTipFactory.FromCard(card);
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
        for (var i = 0; i < _bobOffsets.Length; i++)
            _bobOffsets[i] = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 4f;

        for (var i = 0; i < _iconNodes.Count; i++)
            _iconNodes[i].Position = new Vector2(
                i * IconDistance,
                i < _bobOffsets.Length ? _bobOffsets[i] : 0f);
    }
}
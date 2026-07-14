using Godot;
using Guardian.GuardianCode.Interfaces;

namespace Guardian.GuardianCode.Core;

public partial class CardGemDisplay : Control
{
    private VBoxContainer _slots;

    public CardGemDisplay()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        _slots = new VBoxContainer
        {
            Name = "GemSlots",
            MouseFilter = MouseFilterEnum.Ignore,
            Position = new Vector2(90, -130)
        };
        AddChild(_slots);
    }

    public void Refresh(IGemSocketCard card)
    {
        Visible = card.GemSlots > 0;
        if (!Visible) return;

        foreach (var child in _slots.GetChildren())
        {
            _slots.RemoveChild(child);
            child.QueueFree();
        }

        var gems = card.Gems;
        for (var i = 0; i < card.GemSlots; i++)
        {
            _slots.AddChild(new TextureRect
            {
                Name = $"Slot_{i}",
                Texture = i < gems.Count ? gems[i].Icon : GemModel.EmptyIcon,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(60f, 60f),
                MouseFilter = MouseFilterEnum.Ignore
            });
        }
    }
}
namespace Awakened.AwakenedCode.Interfaces;

public interface ISpell
{
    string SpellIconPath => $"res://Awakened/images/spell_icons/{GetType().Name}.png";
}
namespace Awakened.AwakenedCode.Interfaces;

public interface ISpell
{
    string SpellIconPath => $"res://Awakened/images/spell_icons/Spellbook{GetType().Name}Outline.png";
}
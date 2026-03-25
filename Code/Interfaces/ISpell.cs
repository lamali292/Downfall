namespace Downfall.Code.Interfaces;

public interface ISpell
{
    string SpellIconPath => $"res://Downfall/images/ui/spell_icons/{GetType().Name}.png";
}
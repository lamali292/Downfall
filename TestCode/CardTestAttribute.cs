namespace Downfall.TestCode;

[AttributeUsage(AttributeTargets.Method)]
public class CardTestAttribute(Type? characterType = null, Type? encounterType = null) : Attribute
{
    public Type? CharacterType { get; } = characterType;
    public Type? EncounterType { get; } = encounterType;
}

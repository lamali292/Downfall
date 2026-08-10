using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

/// <summary>One registered custom "package".</summary>
public sealed class CustomPackage
{
    /// <summary>Percent chance (0-100) this package triggers.</summary>
    public int ChancePercent = 5;
    
    public CardModel Card1 = null!;
    public CardModel Card2 = null!;
    public CardModel Card3 = null!;
 
    /// <summary>
    /// Character this package is allowed for. null = every character.
    /// You normally set this via CustomBundleRegistry.Register&lt;T&gt;() rather than by hand.
    /// </summary>
    public Type? Character;
 
    public IReadOnlyList<CardModel> BuildCards()
        => new List<CardModel> { Card1, Card2, Card3 };
 
    public bool MatchesCharacter(CharacterModel character)
        => Character == null || Character.IsInstanceOfType(character);
}


public static class CustomBundleRegistry
{
    public static readonly List<CustomPackage> Packages = new List<CustomPackage>();
 
    /// <summary>Register a package for EVERY character.</summary>
    public static void Register(CustomPackage package) => Packages.Add(package);
 
    /// <summary>
    /// Register a package for one character, with the type checked at the call site:
    /// Register&lt;Defect&gt;(new CustomPackage { ... }).
    /// </summary>
    public static void Register<T>(CustomPackage package) where T : CharacterModel
    {
        package.Character = typeof(T);
        Packages.Add(package);
    }
}

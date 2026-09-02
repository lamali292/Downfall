using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

static class PoolClassifier
{
    private static bool IsUnmoddedPool(CardPoolModel p) =>
        p.GetType().Assembly == typeof(CardPoolModel).Assembly;

    public static bool IsUnmoddedChar(CharacterModel c) =>
        c.GetType().Assembly == typeof(CharacterModel).Assembly;

    public static bool IsDownfallChar(CharacterModel c) => c is DownfallCharacterModel;

    private static bool IsDownfallPool(CardPoolModel p) => p is IDownfallCardPool;

    private static bool IsSameCategory(CardPoolModel pool, Player owner)
    {
        if (IsDownfallChar(owner.Character))
            return IsDownfallPool(pool);
        return !IsUnmoddedChar(owner.Character) || IsUnmoddedPool(pool);
    }
    
    public static bool Allows(PrismaticMode mode, CardPoolModel p, Player owner) =>
        mode switch
        {
            PrismaticMode.All                => true,
            PrismaticMode.VanillaOnly        => IsUnmoddedPool(p),
            PrismaticMode.DownfallOnly       => IsDownfallPool(p),
            PrismaticMode.DownfallAndVanilla => IsUnmoddedPool(p) || IsDownfallPool(p),
            PrismaticMode.Same               => IsSameCategory(p, owner),
            _                                  => true,
        };
}
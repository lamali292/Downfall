using Downfall.DownfallCode.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class StringExtensions
{
    private static string ModId<T>() where T : DownfallCharacterModel =>
        ModelDb.Character<T>().ModId;

    private static string ImgPath(string modId, string subfolder, string file) =>
        Path.Join(modId, "images", subfolder, file);

    private static string WithFallback(string path, string fallback) =>
        ResourceLoader.Exists(path) ? path : fallback;

    private static string FallbackImg(string subfolder, string file) =>
        ImgPath(DownfallMainFile.ModId, subfolder, file);


    public static string CardImageAtlasPath<T>(this string path) where T : DownfallCharacterModel =>
        WithFallback(
            ImgPath(ModId<T>(), "atlases/card_atlas.sprites", path),
            FallbackImg("atlases/card_atlas.sprites", "todo.tres"));

    public static string RestSitePath<T>(this string path) where T : DownfallCharacterModel =>
        ImgPath(ModId<T>(), "ui/restsite", path);

    public static string EnchantmentPath<T>(this string path) where T : DownfallCharacterModel =>
        ImgPath(ModId<T>(), "enchantments", path);

    public static string DownfallPowerImagePath(this string path) =>
        WithFallback(
            FallbackImg("atlases/power_atlas.sprites", path),
            FallbackImg("atlases/power_atlas.sprites", "todo_power.tres"));

    public static string DownfallBigPowerImagePath(this string path) =>
        WithFallback(
            FallbackImg("powers", path),
            FallbackImg("powers", "todo_power.png"));

    public static string PowerImagePath<T>(this string path) where T : DownfallCharacterModel =>
        WithFallback(
            ImgPath(ModId<T>(), "atlases/power_atlas.sprites", path),
            FallbackImg("atlases/power_atlas.sprites", "todo_power.tres"));

    public static string BigPowerImagePath<T>(this string path) where T : DownfallCharacterModel =>
        WithFallback(
            ImgPath(ModId<T>(), "powers", path),
            FallbackImg("powers", "todo_power.png"));

    public static string BigRelicImagePath<T>(this string path) where T : DownfallCharacterModel =>
        WithFallback(
            ImgPath(ModId<T>(), "relics", path),
            FallbackImg("relics", "todo.png"));

    public static string TresRelicImagePath<T>(this string path) where T : DownfallCharacterModel
    {
        var fallbackFile = path.Contains("outline") ? "todo_outline.tres" : "todo.tres";
        return WithFallback(
            ImgPath(ModId<T>(), "atlases/relic_atlas.sprites", path),
            FallbackImg("atlases/relic_atlas.sprites", fallbackFile));
    }
}
using System.Reflection;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.Code.Abstract;

[AttributeUsage(AttributeTargets.Class)]
public class BadgePoolAttribute(string iconPath) : Attribute
{
    public string IconPath { get; } = iconPath;
}

public abstract class CustomBadge(SerializableRun run, ulong playerId) : Badge(run, playerId)
{
    public override string Id => GetType().GetPrefix() + ToUpperSnakeCase(GetType().Name);

    private static string ToUpperSnakeCase(string name)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0)
                sb.Append('_');
            sb.Append(char.ToUpper(name[i]));
        }
        return sb.ToString();
    }

    [HarmonyPatch(typeof(Badge), nameof(Badge.BadgeIcon), MethodType.Getter)]
    private static class BadgeIconPatch
    {
        static bool Prefix(Badge __instance, ref Texture2D __result)
        {
            if (__instance is not CustomBadge badge) return true;
            var attr = badge.GetType().GetCustomAttribute<BadgePoolAttribute>();
            if (attr == null) return true;
            var texture = ResourceLoader.Load<Texture2D>(attr.IconPath);
            if (texture == null) return true;
            __result = texture;
            return false;
        }
    }

    [HarmonyPatch(typeof(NBadge), nameof(NBadge.Create), new[] { typeof(Badge) })]
    private static class NBadgeCreateBadgePatch
    {
        static void Postfix(NBadge __result, Badge badgeModel)
        {
            if (__result == null || badgeModel is not CustomBadge badge) return;
            var attr = badge.GetType().GetCustomAttribute<BadgePoolAttribute>();
            if (attr == null) return;
            var texture = ResourceLoader.Load<Texture2D>(attr.IconPath);
            if (texture == null) return;
            __result.GetNode<TextureRect>("%Icon").Texture = texture;
        }
    }

    [HarmonyPatch(typeof(NBadge), nameof(NBadge.Create), new[] { typeof(string), typeof(BadgeRarity) })]
    private static class NBadgeCreateStringPatch
    {
        static void Postfix(NBadge __result, string id)
        {
            if (__result == null) return;
            var type = DownfallBadgePool.CustomBadgeTypes
                .FirstOrDefault(t => t.GetCustomAttribute<BadgePoolAttribute>() != null &&
                    (GetPrefix(t) + ToUpperSnakeCase(t.Name)).Equals(id, StringComparison.OrdinalIgnoreCase));
            if (type == null) return;
            var attr = type.GetCustomAttribute<BadgePoolAttribute>()!;
            var texture = ResourceLoader.Load<Texture2D>(attr.IconPath);
            if (texture == null) return;
            __result.GetNode<TextureRect>("%Icon").Texture = texture;
        }

        private static string GetPrefix(Type t) => t.GetPrefix();
    }
}

[HarmonyPatch(typeof(BadgePool), nameof(BadgePool.CreateAll))]
public static class DownfallBadgePool
{
    public static readonly List<Type> CustomBadgeTypes = new();

    public static void Initialize(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes()
            .Where(t => !t.IsAbstract
                     && t.IsSubclassOf(typeof(CustomBadge))
                     && t.GetCustomAttribute<BadgePoolAttribute>() != null))
            CustomBadgeTypes.Add(type);
    }

    [HarmonyPostfix]
    static void Postfix(SerializableRun run, ulong playerId, ref IReadOnlyCollection<Badge> __result)
    {
        var list = __result.ToList();
        foreach (var type in CustomBadgeTypes)
            list.Add((Badge)Activator.CreateInstance(type, run, playerId)!);
        __result = list;
    }
}


[BadgePool("res://Downfall/images/powers/aphotic_fount_power.png")]
public class MyBadge(SerializableRun run, ulong playerId) : CustomBadge(run, playerId)
{
    public override BadgeRarity Rarity => BadgeRarity.Silver;
    public override bool RequiresWin => false;
    public override bool MultiplayerOnly => false;
    public override bool IsObtained() => true;
}
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace Downfall.DownfallCode.Voting;

// Lib mod
public static class MainMenuButtonRegistry
{
    private static readonly List<Entry> entries = new();
    public static IReadOnlyList<Entry> Entries => entries;

    public static void Register(Entry entry)
    {
        entries.Add(entry);
    }

    internal static Entry? FindBySubmenuType(Type type)
    {
        return entries.FirstOrDefault(e => e.SubmenuType == type);
    }

    public class Entry
    {
        public Func<NSubmenu?>? CreateSubmenu; // how to build it
        public Func<bool> IsVisible = () => true;
        public required string Label; // or a LocString key
        public Action<NMainMenuSubmenuStack?>? OnPress; // custom action, OR:
        public Type? SubmenuType; // push this submenu
    }
}
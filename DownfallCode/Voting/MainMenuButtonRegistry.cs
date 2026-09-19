using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace Downfall.DownfallCode.Voting;

// Lib mod
public static class MainMenuButtonRegistry
{
	private static readonly List<Entry> entries = new();
	public static IReadOnlyList<Entry> Entries => entries;

	/// Fired every time NMainMenu finishes _Ready (also when returning to the menu).
	public static event Action? MainMenuReady;

	internal static void InvokeMainMenuReady()
	{
		MainMenuReady?.Invoke();
	}

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
		public required string Label; // plain-text fallback; used as-is if LocLabel is null
		public LocString? LocLabel; // preferred: localizable label, takes priority over Label when set
		public Action<NMainMenuSubmenuStack?>? OnPress; // custom action, OR:
		public Type? SubmenuType; // push this submenu

		public string GetDisplayText() => LocLabel?.GetFormattedText() ?? Label;
	}
}

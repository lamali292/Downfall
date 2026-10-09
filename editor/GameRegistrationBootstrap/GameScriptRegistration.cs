using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Bridge;
using MegaCrit.Sts2.Core.Nodes.Ftue;

namespace Downfall.Editor.GameRegistrationBootstrap;

/// <summary>
/// Registers the game assembly's script paths if run in the Editor or Editor-player.
/// </summary>
internal static class GameScriptRegistration
{
    private static bool _registered;
#pragma warning disable CA2255
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Initialize() => EnsureRegistered();

    internal static void EnsureRegistered()
    {
        if (_registered) return;
        _registered = true;
		Register(typeof(NFtue).Assembly);
	}

	private static void Register(Assembly assembly)
	{
		try
		{
			ScriptManagerBridge.LookupScriptsInAssembly(assembly);
		}
		catch (Exception e)
		{
			GD.PushWarning($"[GameRegistrationBootstrap] Could not register script paths for '{assembly.GetName().Name}':\n{e.Message}");
		}
	}
}

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

        // This whole file only ever compiles into the Godot-editor-host build (Downfall.csproj includes
		// "editor/**/*.cs" only when TOOLS is defined - see Downfall.csproj and editor/CLAUDE.md), so there's no
		// "live game" scenario to guard against here - that assembly never contains this type at all.
		GD.Print("[GameRegistrationBootstrap] Looking up the Scripts in the game assembly");
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

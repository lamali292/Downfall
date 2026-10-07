using System.Reflection;
using Godot.Bridge;

namespace Downfall.DownfallCode.Utils;

/// <summary>
/// Every MainFile needs to register its own assembly's script paths with Godot when loaded as a real mod
/// (<see cref="MegaCrit.Sts2.Core.Modding.ModManager"/> loads mods via reflection, so Godot never sees them on its
/// own). <c>DownfallCode</c> itself is excluded from the Godot-editor-host build (see Downfall.csproj and
/// editor/CLAUDE.md), so this is unconditional - no submod's MainFile ever runs as part of that build either.
/// </summary>
public static class ScriptRegistration
{
    public static void Register(Assembly assembly) => ScriptManagerBridge.LookupScriptsInAssembly(assembly);
}

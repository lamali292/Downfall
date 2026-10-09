using System.Reflection;
using Godot.Bridge;

namespace Downfall.DownfallCode.Utils;

public static class ScriptRegistration
{
    public static void Register(Assembly assembly) => ScriptManagerBridge.LookupScriptsInAssembly(assembly);
}

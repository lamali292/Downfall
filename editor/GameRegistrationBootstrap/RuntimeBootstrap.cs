using Godot;

namespace Downfall.Editor.GameRegistrationBootstrap;

/// <summary>
/// Must be the very first entry under <c>project.godot</c>'s [autoload]. <br/>
/// <c>plugin.gd</c> with <c>EditorBootstrap</c> covers the editor itself.
/// </summary>
public partial class RuntimeBootstrap : Node
{
    public RuntimeBootstrap() => GameScriptRegistration.EnsureRegistered();
}

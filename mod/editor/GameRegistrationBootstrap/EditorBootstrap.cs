using Godot;

namespace Downfall.Editor.GameRegistrationBootstrap;

/// <summary>
/// <para>
/// Instantiating this from <c>plugin.gd</c> forces the mod's DLL to load. This runs
/// <see cref="GameScriptRegistration"/>'s <c>[ModuleInitializer] Initialize</c> and registers the game
/// assembly's [ScriptPath] entries before any other editor plugin loads a game script.
/// </para>
/// <para>
/// The game calls Module.GetTypes() on the mod assembly at load time, which throws <c>ReflectionTypeLoadException</c>
/// if any type fails to resolve. -> Do not use anything from <c>GodotSharpEditor.dll</c> here. It is not shipped with the game!
/// </para>
/// </summary>
[Tool]
public partial class EditorBootstrap : RefCounted { }

using System.Reflection;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Encode;

namespace Automaton.AutomatonCode.Functions;

/// <summary>
///     Every Encode and Compile effect a Function knows about. The Function's vars come from here, so the
///     registry must be complete before the game builds the Function card model (<c>ModelDb.Init</c>);
///     mod initializers run before that, so effects are registered from a <c>[ModInitializer]</c>:
///     <see cref="RegisterAssembly" /> picks up every public effect class of an assembly (the Automaton does this
///     for itself, another mod can do it for its own effects), and <see cref="Register(Encodable)" /> /
///     <see cref="Register(Compilable)" /> add a single one.
///     The first read freezes the registry: a later registration would be missing from the Function's vars, so it
///     throws instead of being silently ignored.
/// </summary>
public static class EffectRegistry
{
    private static readonly object Gate = new();
    private static readonly List<Encodable> Encode = [];
    private static readonly List<Compilable> Compile = [];
    private static bool _frozen;

    /// <summary>Every Encode effect, value and non-value, ordered by <see cref="Encodable.Id" />.</summary>
    public static IReadOnlyList<Encodable> Encodables
    {
        get
        {
            lock (Gate)
            {
                _frozen = true;
                return field ??= Encode.OrderBy(e => e.Id, StringComparer.Ordinal).ToList();
            }
        }
    }

    /// <summary>The Encode effects with a value, in play order (<see cref="ValueEncode.Order" />).</summary>
    public static IReadOnlyList<ValueEncode> ValueEncodes
    {
        get
        {
            lock (Gate)
            {
                _frozen = true;
                return field ??= Encode.OfType<ValueEncode>()
                    .OrderBy(e => e.Order).ThenBy(e => e.Id, StringComparer.Ordinal).ToList();
            }
        }
    }

    /// <summary>Every Compile effect, in list order (<see cref="Compilable.Order" />).</summary>
    public static IReadOnlyList<Compilable> Compilables
    {
        get
        {
            lock (Gate)
            {
                _frozen = true;
                return field ??= Compile.OrderBy(c => c.Order).ThenBy(c => c.Id, StringComparer.Ordinal)
                    .ToList();
            }
        }
    }

    /// <summary>Registers one effect. Registering the same effect class twice does nothing.</summary>
    public static void Register(Encodable effect)
    {
        lock (Gate)
        {
            if (Encode.Any(e => e.GetType() == effect.GetType())) return;
            EnsureNotFrozen(effect);
            Encode.Add(effect);
        }
    }

    /// <summary>Registers one effect. Registering the same effect class twice does nothing.</summary>
    public static void Register(Compilable effect)
    {
        lock (Gate)
        {
            if (Compile.Any(c => c.GetType() == effect.GetType())) return;
            EnsureNotFrozen(effect);
            Compile.Add(effect);
        }
    }

    /// <summary>
    ///     Registers every public, concrete <see cref="Encodable" /> and <see cref="Compilable" /> class of
    ///     <paramref name="assembly" /> that has a public parameterless constructor.
    /// </summary>
    public static void RegisterAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsPublic || type.IsAbstract || type.IsGenericTypeDefinition) continue;
            if (type.GetConstructor(Type.EmptyTypes) == null) continue;

            if (type.IsSubclassOf(typeof(Encodable)))
                Register((Encodable)Activator.CreateInstance(type)!);
            else if (type.IsSubclassOf(typeof(Compilable)))
                Register((Compilable)Activator.CreateInstance(type)!);
        }
    }

    private static void EnsureNotFrozen(object effect)
    {
        if (_frozen)
            throw new InvalidOperationException(
                $"{effect.GetType().Name} was registered after the effect registry was first read. " +
                "Register effects from a [ModInitializer], before the game builds its models.");
    }
}

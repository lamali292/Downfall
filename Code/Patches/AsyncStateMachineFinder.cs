using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace Downfall.Code.Patches;

public static class AsyncStateMachineFinder
{
    public static MethodBase PatchAsync(this MethodBase methodBase)
    {
        var stateMachineAttribute = methodBase.GetCustomAttribute<AsyncStateMachineAttribute>();
        if (stateMachineAttribute == null)
            throw new ArgumentException($"{methodBase.FullDescription()} is not an async method");
        return stateMachineAttribute.StateMachineType.GetMethod("MoveNext",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
    }
}
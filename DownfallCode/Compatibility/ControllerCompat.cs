using MegaCrit.Sts2.Core.Nodes.CommonUi;
using System;
using System.Reflection;

namespace Downfall.DownfallCode.Compatibility;

static class ControllerCompat
{
    private static Func<NControllerManager, bool>? _getter;
    private static bool _resolved;

    static void Resolve()
    {
        if (_resolved) return;
        _resolved = true;

        var type = typeof(NControllerManager);

        var prop = type.GetProperty("IsUsingDirectionalNavigation",
                       BindingFlags.Public | BindingFlags.Instance)
                   ?? type.GetProperty("IsUsingController",
                       BindingFlags.Public | BindingFlags.Instance);

        var getMethod = prop?.GetGetMethod();
        if (getMethod != null && prop!.PropertyType == typeof(bool))
        {
            _getter = (Func<NControllerManager, bool>)Delegate.CreateDelegate(
                typeof(Func<NControllerManager, bool>), getMethod);
        }
    }

    public static bool IsUsingController
    {
        get
        {
            Resolve();
            var instance = NControllerManager.Instance;   // direct, no reflection needed
            return _getter != null && instance != null && _getter(instance);
        }
    }
}
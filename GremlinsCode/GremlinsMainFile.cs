using System.Reflection;
using BaseLib.Patches.Saves;
using Downfall.DownfallCode;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Godot;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Gremlins.GremlinsCode;

[ModInitializer(nameof(Initialize))]
public static class GremlinsMainFile
{
    public const string ModId = "Gremlins"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        ExtendedSaveTypes.RegisterObjectSaveType<GremlinSaveData>(
            ExtendedSaveTypes.PropertyFunc<GremlinSaveData, ModelId>("ModelId"),
            ExtendedSaveTypes.PropertyFunc<GremlinSaveData, int>("Hp"),
            ExtendedSaveTypes.PropertyFunc<GremlinSaveData, int>("MaxHp")
        );
        ExtendedSaveTypes.RegisterListSaveType<GremlinSaveData>();
        BundledSubmodLocRegistry.Register(ModId);
        DeathHooks.RegisterInterceptor(GremlinsModel.OnDeath);
    }
   
}
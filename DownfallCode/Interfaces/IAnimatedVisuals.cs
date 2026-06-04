using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Interfaces;

public interface IAnimatedVisuals
{
    void OnAnimationTrigger(string trigger);
}
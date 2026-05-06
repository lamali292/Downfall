using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Hexaghost.HexaghostCode.Vfx;

[GlobalClass]
public partial class NHexaghostCreatureVisuals : NCreatureVisuals, IAnimatedVisuals
{
    public NHexaghostVisuals? Visuals;

    public override void _Ready()
    {
        base._Ready();
        Visuals = GetNode<NHexaghostVisuals>("%Hexaghost");
    }

    

    public void OnAnimationTrigger(string trigger)
    {
        Visuals?.OnAnimationTrigger(trigger);
    }
}
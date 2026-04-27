using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NTopBarEssenceDisplay : NCustomTopBarDisplayElement
{
    public override string ScenePath => "res://Collector/scenes/ui/top_bar_essence.tscn";
    public override float Width => 80f;
    protected override string IconNodePath => "Control";
    protected override string CountLabelNodePath => "EssenceCount";

    public override Func<Player, bool> CanUse =>
        player => player.Character == ModelDb.Character<Core.Collector>();

    protected override int? GetCount()
    {
        return Player?.GetEssence();
    }
}
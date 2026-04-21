using Downfall.Code.Character;
using Downfall.Code.Extensions;
using Downfall.Code.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NTopBarEssenceDisplay : NCustomTopBarDisplayElement
{
    public override string ScenePath => "res://Downfall/scenes/ui/top_bar_essence.tscn";
    public override float Width => 80f;
    protected override string IconNodePath => "Control";
    protected override string CountLabelNodePath => "EssenceCount";

    public override Func<Player, bool> CanUse =>
        player => player.Character == ModelDb.Character<Collector>();

    protected override int? GetCount()
    {
        return Player?.GetEssence();
    }
}
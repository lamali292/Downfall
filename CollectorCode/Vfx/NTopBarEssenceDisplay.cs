using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NTopBarEssenceDisplay : NCustomTopBarDisplayElement
{
    private static NTopBarEssenceDisplay? _instance;

    protected override string IconNodePath => "Control";
    protected override string CountLabelNodePath => "EssenceCount";

    public override void Initialize(Player player)
    {
        base.Initialize(player);
        _instance = this;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
    }

    protected override int? GetCount()
    {
        return Player?.GetEssence();
    }

    public static void RefreshDisplay()
    {
        _instance?.RefreshCount();
    }

    public class Descriptor : ITopBarElementDescriptor
    {
        public string ScenePath => "res://Collector/scenes/ui/top_bar_essence.tscn";
        public float Width => 80f;

        public bool CanUse(Player player)
        {
            return player.Character == ModelDb.Character<Core.Collector>();
        }
    }
}
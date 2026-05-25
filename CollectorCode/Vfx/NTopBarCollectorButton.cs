using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NTopBarCollectorButton : NCustomTopBarButton
{
    private static NTopBarCollectorButton? _instance;

    public static Vector2 ButtonPosition => _instance?.GlobalPosition ?? Vector2.Zero;
    public static Vector2 ButtonSize => _instance?.Size ?? Vector2.Zero;

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

    protected override int? GetCount() =>
        Player == null ? null : CollectiblesModel.GetCollectibles(Player).Count;

    public static void RefreshButton() => _instance?.RefreshCount();

    protected override void OnRelease()
    {
        base.OnRelease();
        if (Player == null || NCapstoneContainer.Instance == null) return;
        if (IsOpen())
            NCapstoneContainer.Instance.Close();
        else
            NCollectiblesViewScreen.ShowScreen(Player, CollectiblesModel.GetCollectibles(Player));
        UpdateScreenOpen();
    }

    protected override bool IsOpen() =>
        NCapstoneContainer.Instance?.CurrentCapstoneScreen is NCollectiblesViewScreen;

    public class Descriptor : ITopBarElementDescriptor
    {
        public string ScenePath => "res://Collector/scenes/ui/top_bar_collector_button.tscn";
        public float Width => 80f;
        public bool CanUse(Player player) => player.Character == ModelDb.Character<Core.Collector>();
    }
}
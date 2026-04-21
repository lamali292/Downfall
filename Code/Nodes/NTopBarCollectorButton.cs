using Downfall.Code.Character;
using Downfall.Code.Core.Collector;
using Downfall.Code.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NTopBarCollectorButton : NCustomTopBarButton
{
    public override string ScenePath => "res://Downfall/scenes/ui/top_bar_collector_button.tscn";
    public override float Width => 80f;

    public override Func<Player, bool> CanUse =>
        player => player.Character == ModelDb.Character<Collector>();

    protected override int? GetCount()
    {
        return Player == null ? null : CollectiblesModel.GetCollectibles(Player).Count;
    }


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

    protected override bool IsOpen()
    {
        return NCapstoneContainer.Instance?.CurrentCapstoneScreen is NCollectiblesViewScreen;
    }
}
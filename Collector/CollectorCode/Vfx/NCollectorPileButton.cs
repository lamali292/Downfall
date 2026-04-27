using Collector.CollectorCode.Piles;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectorPileButton : NCustomCombatCardPile
{
    public override string ScenePath => "res://Collector/scenes/ui/collector_pile.tscn";
    protected override PileType Pile => CollectorPile.Collected;
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(14f, -310f);

    public override Func<Player, bool> CanUsePile =>
        player => player.Character == ModelDb.Character<Core.Collector>();

    protected override LocString BuildEmptyPileMessage()
    {
        return new LocString("combat_messages", "OPEN_EMPTY_COLLECTED");
    }

    protected override HoverTip BuildHoverTip()
    {
        return new HoverTip(
            new LocString("static_hover_tips", "COLLECTOR-COLLECTED_PILE.title"),
            new LocString("static_hover_tips", "COLLECTOR-COLLECTED_PILE.description")
        );
    }
}
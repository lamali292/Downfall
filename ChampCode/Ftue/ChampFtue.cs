using Downfall.DownfallCode.Utils.UI.Ftue;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Ftue;

public static class ChampFtue
{
    public static readonly DownfallFtue.Tip Rules = new("champ_rules_ftue", "CHAMP-RULES_FTUE");

    private const string StanceDanceCardImage = "res://Champ/images/ftue/champ_ftue_1.png";
    private const string StanceCounterImage = "res://Champ/images/ftue/champ_ftue_2.png";
    private const string FinisherImage = "res://Champ/images/ftue/champ_ftue_3.png";

    public static void QueueRules(Player player) =>
        DownfallFtue.QueueRules(Rules, player, StanceDanceCardImage, StanceCounterImage, FinisherImage);
}

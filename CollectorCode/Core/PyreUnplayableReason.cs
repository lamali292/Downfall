using Collector.CollectorCode.Cards;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

internal class PyreUnplayableReason : ICustomUnplayableReason
{
    public UnplayableReason Flag => CollectorUnplayableReason.PyreNoTarget;

    public bool AppliesTo(CardModel card) =>
        card is CollectorCardModel { IsBlockedByMissingPyreTarget: true };

    public LocString GetDialogueLine(CardModel card) =>
        new("gameplay_ui", "PYRE_NO_TARGET");
}

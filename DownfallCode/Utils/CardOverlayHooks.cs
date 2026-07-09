using Downfall.DownfallCode.Patches;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Utils;

// Public entry point for "model changed, refresh my card"
public static class CardOverlayHooks
{
    public static void Refresh(CardModel card)
    {
        var ncard = NCard.FindOnTable(card);
        if (ncard != null) CardOverlayPatches.Sync(ncard);
    }
}
using Downfall.DownfallCode.Interfaces;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public class HighNoonDefendPower : HermitPowerModel, IModifyCardReplayCount
{
    public HighNoonDefendPower()
    {
        WithDefendTip();
    }

    private bool IsMyBasicDefend(CardModel card)
    {
        return card.Owner?.Creature == Owner &&
               card.Tags.Contains(CardTag.Defend) &&
               card.Rarity == CardRarity.Basic;
    }

    // Derived from the live power rather than written into BaseReplayCount, so copies of a card
    // (Juggling, Echo, ...) and cards created later get exactly Amount extra plays, never a stacked grant.
    // Going through GetEnchantedReplayCount also makes the card text show the Replay value.
    public int ModifyReplayCount(CardModel card, int current)
    {
        return IsMyBasicDefend(card) ? current + Amount : current;
    }
}
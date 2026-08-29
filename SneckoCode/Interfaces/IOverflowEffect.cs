using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Snecko.SneckoCode.Interfaces;

/// <summary>
///     Implemented by cards that carry an Overflow payload. This interface only
///     declares that an overflow effect <i>exists</i>, it is NOT how you detect
///     whether a card is an Overflow card or whether overflow should fire.
///     <para>
///         To detect an Overflow card, check for the
///         <see cref="Snecko.SneckoCode.CustomEnums.SneckoKeywords.Overflow" /> keyword
///         (and gate firing on <c>SneckoCmd.OverflowActive(card)</c>). A card may
///         implement this interface without the keyword being present/active, and code
///         that keys off the type instead of the keyword will trigger overflow when it
///         shouldn't.
///     </para>
/// </summary>
public interface IHasOverflowEffect
{
    Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}
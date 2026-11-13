using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Interfaces;

internal interface ISneckoPoolSupplier
{
    /// <summary>The character pools this supplier grants access to (usually one, but Prismatic
    /// Snecko grants every character's pool at once).</summary>
    IEnumerable<CharacterModel> AddSneckoChars();

    /// <summary>How much of the act 1 selection this supplier counts for - a normal SneckoChoice is
    /// worth 1 of Core.SneckoPoolSelection.RoundCount picks; Prismatic Snecko overrides this to
    /// RoundCount so obtaining just the one relic completes the selection immediately.</summary>
    int ActEntryWeight => 1;
}
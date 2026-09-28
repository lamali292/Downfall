using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Relics;

/// <summary>
/// Granted instead of a specific SneckoChoice by picking the "Prismatic Snecko" option during the
/// act 1 pool selection (see SneckoPoolSelection.RunPlayer). Supplies every character's pool at once
/// through the normal ISneckoPoolSupplier hook - no special-casing needed in SneckoModel.
/// </summary>
[Pool(typeof(SneckoRelicPool))]
public class PrismaticSnecko : SneckoRelicModel, ISneckoPoolSupplier
{
    public PrismaticSnecko() : base(RelicRarity.Event)
    {
        WithTips(_ => [HoverTipFactory.Static(SneckoTip.Gift)]);
    }

    public IEnumerable<CharacterModel> AddSneckoChars()
    {
        return ModelDb.AllCharacters.Where(c => c != Owner.Character);
    }

    // Worth a full selection's-worth of picks on its own - see ISneckoPoolSupplier.ActEntryWeight.
    public int ActEntryWeight => SneckoPoolSelection.RoundCount;
}

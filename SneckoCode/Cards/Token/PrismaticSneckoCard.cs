using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Cards;

/// <summary>
/// The "pick every character instead" option shown alongside the two normal SneckoChoice picks in
/// SneckoPoolSelection - see PrismaticSnecko for the relic it grants. Title/description are read
/// straight from that relic rather than duplicated here, same as CharacterCard reading SneckoChoice's.
/// </summary>
[Pool(typeof(TokenCardPool))]
#pragma warning disable
public class PrismaticSneckoCard : ConstructedCardModel,
    IModfyCardDescription
#pragma warning restore
{
    public PrismaticSneckoCard() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithTip(SneckoTip.Gift);
    }

    protected override bool IsPlayable => false;

    public override CardPoolModel VisualCardPool => ModelDb.CardPool<SneckoCardPool>();

    public override string Title => ModelDb.Relic<PrismaticSnecko>().Title.GetFormattedText();

    public LocString ModifyDescription(LocString oldLocString)
    {
        return ModelDb.Relic<PrismaticSnecko>().DynamicDescription;
    }

    public static PrismaticSneckoCard Create()
    {
        var card = ModelDb.Card<PrismaticSneckoCard>().ToMutable();
        if (card is not PrismaticSneckoCard prismaticSneckoCard)
            throw new Exception("PrismaticSneckoCard model is not a PrismaticSneckoCard");
        return prismaticSneckoCard;
    }
}

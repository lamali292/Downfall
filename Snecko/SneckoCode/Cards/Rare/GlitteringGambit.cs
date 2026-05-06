using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class GlitteringGambit : SneckoCardModel
{
    public GlitteringGambit() : base(-1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithVar(new GoldVar(150));
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            IsUpgraded = true,
            Gold = 150
        });
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Eternal);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Add);
    }
}
using BaseLib.Abstracts;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using Champ.ChampCode.Stance;

namespace Champ.ChampCode.Extensions;

public static class ConstructedCardModelExtensions
{
    public static ConstructedCardModel WithDefensiveTip(this ConstructedCardModel card)
    {
        return card.WithTips(e => [ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTip]);
    }

    public static ConstructedCardModel WithBerserkerTip(this ConstructedCardModel card)
    {
        return card.WithTips(e => [ChampModelDb.ChampStance<ChampBerserkerStance>().HoverTip]);
    }

    public static ConstructedCardModel WithUltimateTip(this ConstructedCardModel card)
    {
        return card.WithTips(e => [ChampModelDb.ChampStance<ChampUltimateStance>().HoverTip]);
    }

    public static ConstructedCardModel WithFinisher(this ConstructedCardModel card)
    {
        card.WithTags(ChampTag.Finisher);
        card.WithTip(ChampTip.Finisher);
        return card;
    }


    public static ConstructedCardModel WithEnterBerserker(this ConstructedCardModel card)
    {
        card.WithTags(ChampTag.EnterBerserker);
        card.WithBerserkerTip();
        return card;
    }

    public static ConstructedCardModel WithEnterDefensive(this ConstructedCardModel card)
    {
        card.WithTags(ChampTag.EnterDefensive);
        card.WithDefensiveTip();
        return card;
    }

    public static ConstructedCardModel WithGlory(this ConstructedCardModel card, int baseVal, int upgrade = 0)
    {
        card.WithPower<GloryPower>(baseVal, upgrade);
        card.WithUltimateTip();
        return card;
    }
}
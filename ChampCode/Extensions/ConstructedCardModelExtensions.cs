using BaseLib.Abstracts;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using Champ.ChampCode.Stance;

namespace Champ.ChampCode.Extensions;

public static class ConstructedCardModelExtensions
{

    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithDefensiveTip()
        {
            return card.WithTips(e => ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTips);
        }

        public ConstructedCardModel WithBerserkerTip()
        {
            return card.WithTips(e => ChampModelDb.ChampStance<ChampBerserkerStance>().HoverTips);
        }

        public ConstructedCardModel WithUltimateTip()
        {
            return card.WithTips(e => ChampModelDb.ChampStance<ChampUltimateStance>().HoverTips);
        }

        public ConstructedCardModel WithFinisher()
        {
            card.WithTags(ChampTag.Finisher);
            card.WithTip(ChampTip.Finisher);
            return card;
        }


        public ConstructedCardModel WithEnterBerserker()
        {
            card.WithTags(ChampTag.EnterBerserker);
            card.WithBerserkerTip();
            return card;
        }

        public ConstructedCardModel WithEnterDefensive()
        {
            card.WithTags(ChampTag.EnterDefensive);
            card.WithDefensiveTip();
            return card;
        }

        public ConstructedCardModel WithGlory(int baseVal, int upgrade = 0)
        {
            card.WithPower<GloryPower>(baseVal, upgrade);
            //card.WithUltimateTip();
            return card;
        }
    }
    
   
}
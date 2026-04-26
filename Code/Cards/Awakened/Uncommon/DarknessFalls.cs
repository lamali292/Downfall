using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class DarknessFalls : AwakenedCardModel
{
    public DarknessFalls() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(DownfallTip.Drained);
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Block)));
        WithTip(typeof(StrengthPower));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DarknessFallsPower>(ctx, this, 4);
        await CommonActions.ApplySelf<DarkblessedPower>(ctx, this, 1);
    }
}
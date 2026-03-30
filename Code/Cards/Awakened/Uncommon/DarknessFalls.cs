using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
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
        WithTip(DownfallKeyword.Drained);
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Block)));
        WithTip(typeof(StrengthPower));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DarknessFallsPower>(this, 4);
        await CommonActions.ApplySelf<DarkblessedPower>(this, 1);
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
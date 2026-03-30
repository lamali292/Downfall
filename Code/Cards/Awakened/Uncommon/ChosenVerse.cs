using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class ChosenVerse : AwakenedCardModel
{
    public ChosenVerse() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<ChosenVersePower>(4);
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Block)));
    }
    

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<ChosenVersePower>(this, 2);
        if (power == null) return;
        power.SetBlock(DynamicVars.Power<ChosenVersePower>().IntValue);
        power.CardPlay = cardPlay;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<ChosenVersePower>().UpgradeValueBy(2);
    }
}
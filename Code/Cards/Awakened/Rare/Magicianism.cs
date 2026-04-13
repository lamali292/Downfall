using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Magicianism : AwakenedCardModel
{
    public Magicianism() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Block)));
        WithPower<MagicianismPower>(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<MagicianismPower>(this);
    }
}
using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class StrikeOfGenius : ChampCardModel
{
    public StrikeOfGenius() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(new TooltipSource(HoverTip));
    }

    private static IHoverTip HoverTip(CardModel card)
    {
        return card.IsUpgraded
            ? HoverTipFactory.FromPower<StrikeOfGeniusPlusPower>()
            : HoverTipFactory.FromPower<StrikeOfGeniusPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await CommonActions.ApplySelf<StrikeOfGeniusPlusPower>(ctx, this, 1);
        else
            await CommonActions.ApplySelf<StrikeOfGeniusPower>(ctx, this, 1);
    }
}
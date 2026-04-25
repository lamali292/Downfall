using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class HotStreak : HexaghostCardModel
{
    public HotStreak() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<HotStreakPower>(6, 3);
        WithTip(typeof(SoulBurnPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<HotStreakPower>(ctx, this);
    }
}
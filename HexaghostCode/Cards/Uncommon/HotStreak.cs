using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

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
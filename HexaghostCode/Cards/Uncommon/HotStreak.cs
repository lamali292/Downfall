using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class HotStreak : HexaghostCardModel
{
    public HotStreak() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<HotStreakPower>(6, 3);
        this.WithTip<SoulBurnPower>();
    }
    
    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<HotStreakPower>(ctx, this);
    }
}
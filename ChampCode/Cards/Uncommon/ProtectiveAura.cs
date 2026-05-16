using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ProtectiveAura : ChampCardModel
{
    public ProtectiveAura() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<ProtectiveAuraPower>(4, 2);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ProtectiveAuraPower>(ctx, this);
    }
}
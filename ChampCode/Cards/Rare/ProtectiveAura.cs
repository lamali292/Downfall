using BaseLib.Abstracts;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class ProtectiveAura : ChampCardModel
{
    public ProtectiveAura() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<ProtectiveAuraPower>(3, 2, false);
        WithTip(StaticHoverTip.Block);
        WithTip(ChampTip.Finisher);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ProtectiveAuraPower>(ctx, this);
        
    }
}
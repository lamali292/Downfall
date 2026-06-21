using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class ArenaMastery : ChampCardModel
{
    public ArenaMastery() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<ArenaMasteryBerserkerPower>(1, false);
        this.WithPower<ArenaMasteryDefensivePower>(3, 1, false);
        this.WithBerserkerTip();
        this.WithDefensiveTip();
        WithTip(ChampTip.Finisher);
        this.WithTip<StrengthPower>();
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ArenaMasteryBerserkerPower>(ctx, this);
        await CommonActions.ApplySelf<ArenaMasteryDefensivePower>(ctx, this);
    }
}
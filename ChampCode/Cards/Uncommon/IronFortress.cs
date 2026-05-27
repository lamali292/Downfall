using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class IronFortress : ChampCardModel
{
    public IronFortress() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<DexterityPower>(2);
        this.WithPower<MetallicizePower>(3, 2, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        await CommonActions.ApplySelf<MetallicizePower>(ctx, this);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class LeadByExample : SlimeBossCardModel
{
    public LeadByExample() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<LeadByExamplePower>(2, 1, false);
        WithTip<PotencyPower>();
        WithSlimeTip<BruiserSlime>();
        WithTip(SlimeBossTip.Command);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<LeadByExamplePower>(ctx, this);
    }
}

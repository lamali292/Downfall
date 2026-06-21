using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Gluttony : SlimeBossCardModel
{
    public Gluttony() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<GluttonyPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        this.WithTip<Lick>();
        WithTip(SlimeBossTip.Consume);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<GluttonyPower>(ctx, this);
    }
}
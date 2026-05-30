using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Grow : SlimeBossCardModel
{
    public Grow() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(2);
        WithPower<DexterityPower>(2);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = await SlimeBossCmd.DecreaseSlots(ctx, Owner);
        if (amount <= 0) return;
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class ItLooksTasty : SlimeBossCardModel
{
    public ItLooksTasty() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
        WithTip(SlimeBossTip.Consume);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var weakTargets = SlimeBossCmd.WeakTargets(this, cardPlay);
        return SlimeBossCmd.Consume(ctx, this, weakTargets,
            _ => CommonActions.CardAttack(this, cardPlay).Execute(ctx));
    }
}

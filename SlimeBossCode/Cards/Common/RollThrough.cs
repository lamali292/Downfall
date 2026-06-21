using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class RollThrough : SlimeBossCardModel
{
    public RollThrough() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(6, 1);
        this.WithPower<RollThroughPower>(2, 1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<RollThroughPower>(ctx, this);
    }
}
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ThornWhip : CollectorCardModel
{
    public ThornWhip() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(6, 2);
        this.WithTip<Shiv>();
        WithPower<BruisePower>(3, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<BruisePower>(ctx, this, cardPlay);
        await DownfallCardCmd.GiveCard<Shiv>(Owner, PileType.Hand);
    }
}
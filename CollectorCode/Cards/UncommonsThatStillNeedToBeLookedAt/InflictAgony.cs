using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class InflictAgony : CollectorCardModel
{
    public InflictAgony() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(15, 5);
        WithPower<VulnerablePower>(1, 1);
        WithPower<WeakPower>(1, 1);
        WithPower<MiasmaPower>(1, 1);
        WithVar("WVVal", 1, 1);//Should be the same as the above 3 powers.
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        //if (!cardPlay.Target?.IsAfflicted ?? false)
        //{
        if (cardPlay.Target!.HasPower<WeakPower>())
        {
            await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        }
        if (cardPlay.Target!.HasPower<VulnerablePower>())
        {
            await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        }
        if (cardPlay.Target!.HasPower<MiasmaPower>())
        {
            
        }
    }
}
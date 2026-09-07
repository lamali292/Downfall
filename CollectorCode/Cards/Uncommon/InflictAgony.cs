using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
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
        WithVar("Power", 1, 1);
        WithTip<WeakPower>();
        WithTip<VulnerablePower>();
        WithTip<MiasmaPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var amount = DynamicVars["Power"].IntValue;
        if (!cardPlay.Target!.HasPower<WeakPower>())
        {
            await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this, amount);
        }
        if (!cardPlay.Target!.HasPower<VulnerablePower>())
        {
            await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this, amount);
        }
        if (!cardPlay.Target!.HasPower<MiasmaPower>())
        {
            await CommonActions.Apply<MiasmaPower>(ctx, cardPlay.Target, this, amount);
        }
    }
}
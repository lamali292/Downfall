using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
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
        WithTorchheadDamage(15, 1);
        WithVar("Power", 1, 1);
        WithTip<WeakPower>();
        WithTip<VulnerablePower>();
        WithTip<BlisterPower>();
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.Attack(ctx, this, cardPlay);
        var amount = DynamicVars["Power"].IntValue;
        if (!cardPlay.Target!.HasPower<WeakPower>())
        {
            await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this, amount);
        }
        if (!cardPlay.Target!.HasPower<VulnerablePower>())
        {
            await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this, amount);
        }
        if (!cardPlay.Target!.HasPower<BlisterPower>())
        {
            await CommonActions.Apply<BlisterPower>(ctx, cardPlay.Target, this, amount);
        }
    }
}
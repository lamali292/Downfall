using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class ForgeContract : CollectorCardModel
{
    public ForgeContract() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithTorchheadDamage(6, 2);
    }
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CollectorCmd.TorchheadAttack(this).ExecuteIfPresent(ctx);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        var shouldTargetAll = _owner != null && CollectorHook.ShouldTorchheadTargetAll(_owner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}
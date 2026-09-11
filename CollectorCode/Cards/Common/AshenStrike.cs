using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class AshenStrike : CollectorCardModel
{
    // rename
    public AshenStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.Self)
    {
        WithTorchheadDamage(14, 5);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithTags(CardTag.Strike);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.TorchheadAttack(this).ExecuteIfPresent(ctx);
        if (IsUpgraded)
            await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand);
        else 
            await DownfallCardCmd.GiveCard<Burn>(Owner, PileType.Hand);
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        var shouldTargetAll = _owner != null && CollectorHook.ShouldTorchheadTargetAll(_owner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}
using BaseLib.Utils;
using Collector.CollectorCode.Core;
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
    public AshenStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithTorchheadDamage(14, 3);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithTags(CardTag.Strike);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
        if (IsUpgraded)
            await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand);
        else 
            await DownfallCardCmd.GiveCard<Burn>(Owner, PileType.Hand);
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("TorchheadTargetsAll", ShouldTorcheadTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}
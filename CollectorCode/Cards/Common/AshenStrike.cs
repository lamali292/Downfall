using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class AshenStrike : CollectorCardModel
{
    // rename
    public AshenStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithTorchheadDamage(14, 2);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithTags(CardTag.Strike);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.AttackOrKindle(ctx, this, cardPlay);
        if (IsUpgraded)
            await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand);
        else 
            await DownfallCardCmd.GiveCard<Burn>(Owner, PileType.Hand);
    }
}
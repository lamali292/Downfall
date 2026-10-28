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
    public AshenStrike() : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithTorchheadDamage(5, 2);
        WithCards(2);
        WithRepeat(2);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithTags(CardTag.Strike);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.IsTorchheadAlive)
        {
            for (var v = 0; v > DynamicVars.Repeat.IntValue; v++)
            {
                await TorchheadCmd.AttackOrKindle(ctx, this, cardPlay);
            }
        }
        if (IsUpgraded)
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
        else 
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
    }
}
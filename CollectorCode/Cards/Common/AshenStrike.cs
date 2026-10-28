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
    public AshenStrike() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(5, 2);
        WithRepeat(2);
        WithTags(CardTag.Strike);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    

    /// <summary>Torchhead hits what you pick; while it targets ALL enemies (Equip Axe) there is nothing to pick.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (var v = 0; v < DynamicVars.Repeat.IntValue; v++)
            await TorchheadCmd.Attack(ctx, this, cardPlay);
    }
}
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class BramblesparKindling : CollectorCardModel
{
    public BramblesparKindling() : base(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Smolder);
        WithKindle(3, 1);
        WithTorchheadDamage(6, 3);
        WithTip(CollectorKeyword.Torchhead);
    }

    /// <summary>Torchhead hits what you pick; while it targets ALL enemies (Equip Axe) there is nothing to pick.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.AnyEnemy;

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnSmolder(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        await TorchheadCmd.Attack(ctx, this);
    }
}
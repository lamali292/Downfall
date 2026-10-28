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
    public BramblesparKindling() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithKindle(5, 2);
        WithTorchheadDamage(10, 4);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnFlicker(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        await TorchheadCmd.Attack(ctx, this);
    }
}
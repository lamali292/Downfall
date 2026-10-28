using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class IronbarkKindling : CollectorCardModel
{
    public IronbarkKindling() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Flicker);
        WithBlock(4, 2);
        WithKindle(4, 1);
        //WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnFlicker(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        await DownfallCreatureCmd.GainBlock(Owner.Creature, this);
    }
}
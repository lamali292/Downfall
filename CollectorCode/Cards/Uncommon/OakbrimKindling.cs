using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class OakbrimKindling : CollectorCardModel
{
    public OakbrimKindling() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithCards(2, 1);
        WithKindle(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnFlicker(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        await CommonActions.Draw(this, ctx);
    }
}
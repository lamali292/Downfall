using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RotwoodKindling : CollectorCardModel
{
    public RotwoodKindling() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithUpgradingCardTip<Rotshroom>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnFlicker(PlayerChoiceContext ctx)
    {
        await DownfallCardCmd.GiveCard<Rotshroom>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }
}
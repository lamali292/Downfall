using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SunbloomKindling : CollectorCardModel
{
    public SunbloomKindling() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithKindle(1, 3);
        WithPower<ReserveNextTurnPower>(2, false);
        WithReserveTip();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            await TorchheadCmd.Kindle(ctx, this);
            await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
        }
    }
}
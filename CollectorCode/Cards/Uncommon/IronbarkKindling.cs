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
public class IronbarkKindling : CollectorCardModel, ISkipReplayOnSelfExhaust
{
    public IronbarkKindling() : base(3, CardType.Status, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithBlock(5, 2);
        WithKindle(4, 1);
        //WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            await CollectorCmd.Kindle(ctx, this);
            await DownfallCreatureCmd.GainBlock(Owner.Creature, this);
        }

      
    }
}
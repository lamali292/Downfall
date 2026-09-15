using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RotwoodKindling : CollectorCardModel, ISkipReplayOnSelfExhaust
{
    public RotwoodKindling() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithKindle( 1);
        WithTip(new TooltipSource(card =>
        {
            var beam = ModelDb.GetById<MenacingMushrooms>(ModelDb.Card<MenacingMushrooms>().Id).ToMutable();
            if (card.IsUpgraded) beam.UpgradeInternal();
            return HoverTipFactory.FromCard(beam);
        }));
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this || CombatState == null) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            await CollectorCmd.Kindle(ctx, this);
            await CommonActions.Apply<MiasmaPower>(ctx, CombatState.HittableEnemies, this);
            await DownfallCardCmd.GiveCard<MenacingMushrooms>(Owner, PileType.Hand, upgraded: IsUpgraded);
        }
    }
}
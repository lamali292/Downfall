using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class WaterfallGiantCard : Collectible<WaterfallGiantBoss>, ISkipReplayOnSelfExhaust
{
    public WaterfallGiantCard() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, 0.88f)
    {
        
        WithPower<WaterfallGiantCardPower>(3, false);
        WithPower<MiasmaPower>(40, 10);
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            (await CommonActions.ApplySelf<WaterfallGiantCardPower>(ctx, this))?.SetMiasma(DynamicVars.Power<MiasmaPower>().BaseValue);
        }
    }

    //protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay) {}
}
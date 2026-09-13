using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Collectibles;

public class TerrorEelCard
    : Collectible<TerrorEelElite>, ISkipReplayOnSelfExhaust
{
    public TerrorEelCard() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        WithPower<VulnerablePower>(3, 6);
    }
    
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this || CombatState == null) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            await CommonActions.Apply<VulnerablePower>(ctx, CombatState.HittableEnemies, this);
        }
    }
}
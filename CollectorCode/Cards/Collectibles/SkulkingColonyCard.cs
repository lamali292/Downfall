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

public class SkulkingColonyCard : Collectible<SkulkingColonyElite>, ISkipReplayOnSelfExhaust
{
    public SkulkingColonyCard() : base(5, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.3f)
	
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        // todo: we do plating now. idk if thats good conversion of numbers.
        WithPower<PlatingPower>(5, 3);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            await CommonActions.ApplySelf<PlatingPower>(ctx, this);
        }
    }
}
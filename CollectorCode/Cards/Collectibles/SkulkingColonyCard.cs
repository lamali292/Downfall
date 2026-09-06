using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;
public class SkulkingColonyCard : Collectible<SkulkingColonyElite>
{
    public SkulkingColonyCard() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<PlatingPower>(4, 2);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }
}
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;
public class SkulkingColonyCard : Collectible<SkulkingColonyElite>
{
    public SkulkingColonyCard() : base(12, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<PlatedArmorPower>(6, 4);
        //If not giving to torch head, value could be higher, then I thought it would be OP, so I made it pseudo-unplayable.
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }
}
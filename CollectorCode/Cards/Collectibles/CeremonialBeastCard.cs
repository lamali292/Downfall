using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class CeremonialBeastCard : Collectible<CeremonialBeastBoss>
{
    public CeremonialBeastCard() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self, 0.3f)
    {
        WithCalculatedBlock(0, 2, BlockCalc, 0, 1);
        WithPower<CeremonialBeastCardPower>(1, false);
    }

    private static decimal BlockCalc(CardModel card, Creature? creature)
    {
        return card.Owner.PlayerCombatState?.Energy ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<CeremonialBeastCardPower>(ctx, this);
    }
}

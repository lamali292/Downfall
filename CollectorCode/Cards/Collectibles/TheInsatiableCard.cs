using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class TheInsatiableCard : Collectible<TheInsatiableBoss>
{
    public TheInsatiableCard() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, 0.3f)
    {
        WithPower<TheInsatiableCardPower>(3, 1, false);
        WithTip(CollectorTip.Kindle);
        WithTip(CollectorTip.Pyred);
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithEnergyTip();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<TheInsatiableCardPower>(ctx, this);
    }
}

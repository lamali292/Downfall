using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class InfestedPrismCard : Collectible<InfestedPrismsElite>
{
    public InfestedPrismCard() : base(3, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithPower<InfestedPrismCardPower>(2, false);
        WithTip<StrengthPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<InfestedPrismCardPower>(ctx, this);
    }
}


using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;


[Pool(typeof(CollectorCardPool))]
public class AshesToAshes : CollectorCardModel
{
    public AshesToAshes() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<AshesToAshesPower>(1, 1, false);
        WithTip<Ember>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<AshesToAshesPower>(ctx, this);
    }
}
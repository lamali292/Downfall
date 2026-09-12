using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class ReptoCloth : ActsFromThePastCard
{
    public ReptoCloth() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, "REPTOMANCER_ELITE")
    {
        WithKindle(3, 1);
        WithTip<PoisonPower>();
        WithPower<EquipDaggerPower>(2, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CollectorCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<EquipDaggerPower>(ctx, this);
    }
}
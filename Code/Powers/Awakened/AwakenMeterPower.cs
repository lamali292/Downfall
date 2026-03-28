using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

#pragma warning disable STS001
public class AwakenMeterPower : AwakenedPowerModel
#pragma warning restore STS001
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card.Type != CardType.Power) return;
        if (AwakenedCmd.IsAwakened(Owner)) return;

        await PowerCmd.Apply<AwakenMeterPower>(Owner, 1, Owner, null);

        if (Amount >= 8)
            await AwakenedCmd.Awaken(Owner.Player!, ctx);
    }
}
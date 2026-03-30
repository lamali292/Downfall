using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Awakened;

public class CuriosityPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Owner.Player != cardPlay.Card.Owner || cardPlay.Card.Type != CardType.Power) return;
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}

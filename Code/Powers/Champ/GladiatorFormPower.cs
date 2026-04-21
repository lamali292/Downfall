using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Champ;

public class GladiatorFormPower : ChampPowerModel
{
    /*
     * old GladiatorFormPower
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        var powerReceivedEntries = CombatManager.Instance.History.Entries
            .Where(e => e.RoundNumber == combatState.RoundNumber - 1)
            .OfType<PowerReceivedEntry>().ToList();
        var vigorSpent = powerReceivedEntries
            .Where(e => e is { Power: VigorPower, Amount: < 0} && e.Power.Owner == Owner)
            .Sum(e => (int)Math.Abs(e.Amount));

        var counterSpent = powerReceivedEntries
            .Where(e => e is { Power: CounterPower, Amount: < 0 } && e.Power.Owner == Owner)
            .Sum(e => (int)Math.Abs(e.Amount));
        var vigorFromVigor   = Amount * vigorSpent   / 3;
        var counterFromCounter = Amount * counterSpent / 3;
        if (vigorFromVigor > 0)
            await PowerCmd.Apply<VigorPower>(Owner, vigorFromVigor, Owner, null);
        if (counterFromCounter > 0)
            await PowerCmd.Apply<CounterPower>(Owner, counterFromCounter, Owner, null);
        if (counterFromCounter > 0 || vigorFromVigor > 0)
            Flash();
    }
    */

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!props.IsPoweredAttack() || cardSource == null || cardSource.Owner.Creature != Owner)
            return 1M;

        var attacksPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted
            .Count(e => e.HappenedThisTurn(CombatState)
                        && e.CardPlay.Card.Type == CardType.Attack
                        && e.CardPlay.Card.Owner.Creature == Owner);

        var isCurrentCardBeingPlayed = cardSource.Pile?.Type == PileType.Play ? 1 : 0;
        var attackIndex = attacksPlayedThisTurn - isCurrentCardBeingPlayed;

        return attackIndex < Amount ? 2M : 1M;
    }
}
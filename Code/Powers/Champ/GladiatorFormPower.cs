using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils;
using BaseLib.Utils.Patching;
using Downfall.Code.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Champ;

public class GladiatorFormPower : ChampPowerModel
{
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
}
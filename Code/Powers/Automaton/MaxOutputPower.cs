using Downfall.Code.Abstract;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.Code.Powers.Automaton;

public class MaxOutputPower : AutomatonPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (Owner.Player != player || Owner.Player == null) return;

        var canonical = ModelDb.Card<Dazed>();
        var dazed = Enumerable.Range(0, Amount)
            .Select(_ => canonical)
            .ToList<CardModel>();

        await DownfallCardCmd.Insert(dazed, Owner.Player);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player) return count;
        return count + Amount;
    }
}
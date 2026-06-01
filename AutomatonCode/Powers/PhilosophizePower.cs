using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Powers;

public class PhilosophizePower : AutomatonPowerModel, IAfterCompilingFunction
{
    public PhilosophizePower() : base(PowerType.Debuff)
    {
        WithTip<StrengthPower>();
    }


    public async Task AfterCompilingFunction(PlayerChoiceContext ctx, Player player, CardPileAddResult result)
    {
        if (player.Creature != Owner) return;
        var enemies = CombatState.HittableEnemies;
        await PowerCmd.Apply<StrengthPower>(ctx, enemies, Amount,
            Owner, null);
        await PowerCmd.Remove(this);
    }
}
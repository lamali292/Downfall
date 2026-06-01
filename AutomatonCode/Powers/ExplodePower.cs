using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Powers;

public class ExplodePower : AutomatonPowerModel, IAfterCompilingFunction
{
    public ExplodePower() : base(PowerType.Debuff)
    {
        WithTip<Burn>();
    }

    public async Task AfterCompilingFunction(PlayerChoiceContext ctx, Player player, CardPileAddResult result)
    {
        if (player.Creature == Owner) return;
        await DownfallCardCmd.GiveCards<Burn>(player, PileType.Draw, Amount);
        await PowerCmd.Remove(this);
    }
}
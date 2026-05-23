using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Automaton.AutomatonCode.Powers;

[Obsolete]
public class SentientFormPower : AutomatonPowerModel
{

    public SentientFormPower()
    {
        WithTip(typeof(StrengthPower));
    }
    
    
    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner)
            return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
    }
}
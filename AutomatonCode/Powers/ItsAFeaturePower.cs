using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Powers;

public class ItsAFeaturePower : AutomatonPowerModel
{
    public ItsAFeaturePower()
    {
        WithTip(typeof(VigorPower));
    }

    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
    }
}
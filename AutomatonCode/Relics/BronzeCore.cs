using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class BronzeCore() : AutomatonRelicModel(RelicRarity.Starter), IOnCompile
{
    private bool _isTriggered;

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<PlatinumCore>();
    }

    public override Task BeforeCombatStart()
    {
        _isTriggered = false;
        return base.BeforeCombatStart();
    }


    public async Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<CardModel> snapshot, FunctionCard functionCard, CardPlay cardPlay)
    {
        if (functionCard.Owner == Owner && !_isTriggered)
        {
            Flash();
            _isTriggered = true;
            await PlayerCmd.GainEnergy(1, Owner);
        }
    }

}
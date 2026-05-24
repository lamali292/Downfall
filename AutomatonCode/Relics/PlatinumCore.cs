using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class PlatinumCore : AutomatonRelicModel
{
    private int _triggeredCount;

    public PlatinumCore() : base(RelicRarity.Starter)
    {
        WithCards(3);
    }

    public int TriggeredCount
    {
        get => _triggeredCount;
        set
        {
            AssertMutable();
            _triggeredCount = value;
            Status = value == DynamicVars.Cards.IntValue ? RelicStatus.Disabled : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    public override int DisplayAmount => DynamicVars.Cards.IntValue - TriggeredCount;

    public async Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<CardModel> snapshot, FunctionCard functionCard,
        CardPlay cardPlay)
    {
        if (functionCard.Owner == Owner && TriggeredCount < DynamicVars.Cards.IntValue)
        {
            Flash();
            TriggeredCount++;
            functionCard.EnergyCost.SetUntilPlayed(0);
        }
    }

    public override Task BeforeCombatStart()
    {
        TriggeredCount = 0;
        return Task.CompletedTask;
    }
}
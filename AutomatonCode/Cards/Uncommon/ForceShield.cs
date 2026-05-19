using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ForceShield : AutomatonCardModel
{
    private int _functionsCreated;

    public ForceShield() : base(4, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12, 4);
        WithEnergyTip();
    }

    public override bool ShouldReceiveCombatHooks => true;

    public Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<AutomatonCardModel> snapshot,
        FunctionCard functionCard, CardPlay cardPlay)
    {
        _functionsCreated++;
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }


    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card != this)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = Math.Max(0, originalCost - _functionsCreated);
        return modifiedCost != originalCost;
    }
}
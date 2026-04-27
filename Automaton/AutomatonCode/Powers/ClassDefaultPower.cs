using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class ClassDefaultPower : AutomatonPowerModel, IOnCompile
{
    public override bool ShouldReceiveCombatHooks => true;

    public async Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<AutomatonCardModel> snapshot,
        FunctionCard functionCard, CardPlay cardPlay)
    {
        if (Amount <= 0 || Owner.Player == null) return;
        var pile = AutomatonCmd.GetEncodePile(Owner.Player);
        if (pile == null) return;
        var copy = Owner.CombatState!.CloneCard(cardPlay.Card);
        if (copy is IEncodable encodable) await encodable.Encode(ctx, cardPlay);
        await PowerCmd.Decrement(this);
    }
}
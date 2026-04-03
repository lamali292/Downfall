using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Automaton;

public class ClassDefaultPower : AutomatonPowerModel, IOnCompile
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

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
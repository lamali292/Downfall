using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Automaton;

public class SummonOrbPower : AutomatonPowerModel, IOnCompile
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnCompile(PlayerChoiceContext ctx, IReadOnlyList<AutomatonCardModel> snapshot,
        FunctionCard functionCard,
        CardPlay cardPlay)
    {
        if (functionCard.Owner != Owner.Player) return;
        await ExecuteCreatureCommands(ctx, cardPlay);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card is not FunctionCard) return;
        await ExecuteCreatureCommands(ctx, cardPlay);
    }

    private async Task ExecuteCreatureCommands(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
        await DamageCmd.Attack(Amount)
            .Unpowered()
            .FromCard(cardPlay.Card)
            .TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
}
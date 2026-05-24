using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Goto : AutomatonCardModel
{
    public Goto() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(7, 1);
        WithCards(1, 1);
        WithEnergy(0);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Draw(this, ctx);
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != Owner || card.Owner != Owner || card.Type != CardType.Status) return Task.CompletedTask;
        EnergyCost.SetUntilPlayed(DynamicVars.Energy.IntValue);
        return Task.CompletedTask;
    }
}
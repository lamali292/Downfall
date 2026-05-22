using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class Gambit : HermitCardModel
{
    public Gambit() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var combatCardSelection = Owner.RunState.Rng.CombatCardSelection;
        var cards = Owner.GetDiscard()
            .Where(c => c.Type == CardType.Attack)
            .TakeRandom(DynamicVars.Cards.IntValue, combatCardSelection)
            .ToList();
        await CardPileCmd.Add(cards, PileType.Hand);
        foreach (var card in cards) card.EnergyCost.AddThisTurn(-1, true);
    }
}
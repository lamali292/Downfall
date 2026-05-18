using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Exhaust ALL Unplayable cards from other players. Add ALL exhausted curse cards to your hand.
///     Upgrade: Retain.
/// </summary>
public sealed class DebtsOfSin : HermitCardModel
{
    public DebtsOfSin() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip(CardKeyword.Unplayable);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var players = CombatState!.GetTeammatesOf(Owner.Creature)
            .Where(c => c is { IsAlive: true, IsPlayer: true, Player: not null } && c.Player != Owner);

        foreach (var creature in players)
        {
            foreach (var pileType in new[] { PileType.Hand, PileType.Discard, PileType.Draw })
            {
                var unplayableCards = pileType.GetPile(creature.Player!).Cards
                    .Where(c => c.Keywords.Contains(CardKeyword.Unplayable))
                    .ToList();

                foreach (var card in unplayableCards)
                {
                    await CardCmd.Exhaust(ctx, card);
                    if (card.Type != CardType.Curse) continue;
                    var newCard = CombatState.CreateCard(card, Owner);
                    await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, Owner);
                }
            }
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithKeyword(CardKeyword.Exhaust), WithKeyword(CardKeyword.Retain, UpgradeType.Add)
 */
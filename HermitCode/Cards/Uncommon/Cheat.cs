using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Look at the top 3 cards in your draw pile. Choose one to play.
///     Dead On: Also trigger its Dead On effect.
///     Upgrade: Top 5 cards.
/// </summary>
public sealed class Cheat : HermitCardModel
{
    public Cheat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithCards(3, 2);
    }

   

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var drawPile = PileType.Draw.GetPile(Owner);
        var topCards = drawPile.Cards.Take(DynamicVars.Cards.IntValue).ToList();
        if (topCards.Count == 0)
            return;

        var selected = (await CardSelectCmd.FromSimpleGrid(
            ctx,
            topCards,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1)
        )).FirstOrDefault();

        if (selected == null)
            return;

        if (IsDeadOn)
            await PowerCmd.Apply<CheatPower>(ctx, Owner.Creature, 1, Owner.Creature, this, true);

        await CardCmd.AutoPlay(ctx, selected, null);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithCards(3, 2)
 */
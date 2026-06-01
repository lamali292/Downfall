using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Cheat : HermitCardModel, IHasDeadOnEffect
{
    public Cheat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(3, 2);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var isDeadOn = PatchDeadOnCapture.LastWasDeadOn;
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

        if (isDeadOn) await PowerCmd.Apply<CheatPower>(ctx, Owner.Creature, 1, Owner.Creature, this, true);
        await CardCmd.AutoPlay(ctx, selected, null);
    }
}
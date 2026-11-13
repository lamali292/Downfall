using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Cheat : HermitCardModel
{
    public Cheat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(3, 2);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        // Snapshot taken by DeadOnPatch when this card started playing; per-card, so the
        // auto-played card below can't clobber it.
        var isDeadOn = HermitCmd.IsDeadOn(this);
        await HermitCmd.DeadOn(ctx, this, play);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        var topCards = Owner.DrawPile.Take(DynamicVars.Cards.IntValue).ToList();
        if (topCards.Count == 0)
            return;

        var selected = (await CardSelectCmd.FromSimpleGrid(
            ctx,
            topCards,
            Owner,
            new CardSelectorPrefs(DownfallCardSelectorPrefs.PlaySelectionPrompt, 1)
        )).FirstOrDefault();

        if (selected == null)
            return;

        if (isDeadOn) await PowerCmd.Apply<CheatPower>(ctx, Owner.Creature, 1, Owner.Creature, this, true);

        await CardCmd.AutoPlay(ctx, selected, null);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Basic;

public sealed class Covet : HermitCardModel
{
    public Covet() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithCards(1, 1);
        WithVar("Discard", 1);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars["Discard"].IntValue);
        var selected = (await CardSelectCmd.FromHandForDiscard(
            ctx,
            Owner,
            prefs,
            null,
            this
        )).FirstOrDefault();
        if (selected != null)
        {
            if (selected.Type == CardType.Curse)
                await CardCmd.Exhaust(ctx, selected);
            else
                await CardCmd.Discard(ctx, selected);
        }

        await CommonActions.Draw(this, ctx);
    }
}
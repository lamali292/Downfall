using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
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
        )).ToList();
        await CardCmd.Discard(ctx, selected);
        foreach (var card in selected.Where(e => e.Type == CardType.Curse))
        {
            await CardCmdCompatibility.Exhaust(ctx, card);
        }
        await CommonActions.Draw(this, ctx);
    }
}
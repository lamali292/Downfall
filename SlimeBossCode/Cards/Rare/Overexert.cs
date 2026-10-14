using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Overexert : SlimeBossCardModel
{
    public Overexert() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override bool HasEnergyCostX => true;

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        var drawAmount = x + (IsUpgraded ? 1 : 0);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var drawn = await CardPileCmd.Draw(ctx, drawAmount, Owner);

        // The X energy is already spent, so without this the drawn cards would show as unplayable in the picker.
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.PlaySelectionPrompt, 1)
        {
            PretendCardsCanBePlayed = true
        };
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, c => drawn.Contains(c), this)).FirstOrDefault();
        if (card == null) return;

        // The second play uses a copy: a Power card resolves out of every pile after its first play, and the game's
        // unplayable-checks would then reject a second AutoPlay of the same instance.
        var dupe = card.CreateDupeCompat();
        card.ExhaustOnNextPlay = true;
        await CardCmd.AutoPlay(ctx, card, null);
        await CardCmd.AutoPlay(ctx, dupe, null);
    }
}

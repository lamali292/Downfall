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
    private CardModel? _cardToReplay;

    public Overexert() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override bool HasEnergyCostX => true;

    protected override Artist Artist => Artist.Get<Opal>();

    // Overexert is itself a live combat hook listener while its own OnPlayInternal is resolving (it's
    // still sitting in the Play pile), so it can bump just the chosen card's play count directly - no
    // separate modifier/power needed. Calling CardCmd.AutoPlay twice on the same instance instead would
    // silently no-op the second time for Power cards (they resolve to PileType.None after playing once,
    // which the game's own unplayable-checks then reject).
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card == _cardToReplay ? playCount + 1 : playCount;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        var drawAmount = x + (IsUpgraded ? 1 : 0);
        var drawn = await CardPileCmd.Draw(ctx, drawAmount, Owner);

        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.PlaySelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, c => drawn.Contains(c), this)).FirstOrDefault();
        if (card == null) return;

        _cardToReplay = card;
        await CardCmd.AutoPlay(ctx, card, null);
        _cardToReplay = null;

        if (card.Pile != null) await CardCmdCompatibility.Exhaust(ctx, card);
    }
}

using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class QuickSnack : SlimeBossCardModel
{
    public QuickSnack() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2, 1);
        WithKeyword(CardKeyword.Exhaust);
        WithTip<Slimed>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (card == null) return;
        var wasSlimed = card is Slimed;
        await CardCmdCompatibility.Exhaust(ctx, card);
        if (wasSlimed) await CommonActions.Draw(this, ctx);
    }
}

using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class QuickSnack : SlimeBossCardModel
{
    public QuickSnack() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithKeyword(CardKeyword.Exhaust);
        this.WithTip<Lick>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (card == null) return;
        var amount = card.EnergyCost.GetResolved();
        await CardCmd.Exhaust(ctx, card);
        await DownfallCardCmd.GiveCards<Lick>(Owner, PileType.Hand, amount);
    }
}
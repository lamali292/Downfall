using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Reformation : SlimeBossCardModel
{
    public Reformation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(6, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var hand = Owner.Hand.ToList();
        await CardPileCmd.Add(hand, PileType.Discard);

        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ToHandSelectionPrompt, 1);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Discard.GetPile(Owner), Owner, prefs);
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}

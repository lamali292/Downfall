using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Reformation : SlimeBossCardModel
{
    public Reformation() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithCards(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ToTopSelectionPrompt, DynamicVars.Cards.IntValue);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Discard.GetPile(Owner), Owner, prefs);
        await CardPileCmd.Add(cards, PileType.Draw, CardPilePosition.Top);
    }
}
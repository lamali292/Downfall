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
public class NewPlan : SlimeBossCardModel
{
    public NewPlan() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ToTopSelectionPrompt, 1);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Hand.GetPile(Owner), Owner, prefs);
        await CardPileCmd.Add(cards, PileType.Draw, CardPilePosition.Top);
    }
}

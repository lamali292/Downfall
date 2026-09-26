using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Mitosis : SlimeBossCardModel
{
    public Mitosis() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Copies", 1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = await DownfallCardSelectionCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.ToTopSelectionPrompt,
            1, this, c => c != this, true);
        var card = cards.FirstOrDefault();
        if (card == null) return;
        var copies = DynamicVars["Copies"].IntValue;
        for (var i = 0; i < copies; i++)
            await CardPileCmd.Add(card.CreateClone(), PileType.Hand);
    }
}

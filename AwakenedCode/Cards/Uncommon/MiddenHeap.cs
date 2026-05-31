using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class MiddenHeap : AwakenedCardModel
{
    public MiddenHeap() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(3, 1);
        WithCards(1, 1);
    }

    protected override Artist Artist => Artist.Get<Occultpyromancer>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var cardsToSelect = Owner.GetDiscard()
            .Concat(Owner.GetDraw()).Where(c => c.Type is CardType.Status or CardType.Curse)
            .ToList();
        var selected = await DownfallCardCmd.SelectFromCards(ctx, cardsToSelect,
            DownfallCardSelectorPrefs.ToHandSelectionPrompt, this);
        foreach (var cardModel in selected) await CardPileCmd.Add(cardModel, PileType.Hand);
    }
}
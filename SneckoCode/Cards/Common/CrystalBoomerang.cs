using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class CrystalBoomerang : SneckoCardModel
{
    public CrystalBoomerang() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(5, 3);
        WithCards(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = (await DownfallCardCmd.SelectFromCards(ctx, Owner.GetDiscard(),
            DownfallCardSelectorPrefs.ToHandSelectionPrompt, this, true)).FirstOrDefault();
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Hand);
        if (SneckoCmd.IsOffclass(this, card))
            await CommonActions.CardBlock(this, cardPlay);
    }
}
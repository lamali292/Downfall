using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class SeventhEye : HexaghostCardModel
{
    public SeventhEye() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Claude27A>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = (await DownfallCardCmd.SelectFromCards(ctx, Owner.GetDraw(),
            DownfallCardSelectorPrefs.ToHandSelectionPrompt, this)).FirstOrDefault();
        if (card != null) await CardPileCmd.Add(card, PileType.Hand);
        await HexaghostCmd.MoveToRandom(ctx, Owner, true);
        await HexaghostCmd.ReplaceCurrentWithRandom(Owner);
    }
}
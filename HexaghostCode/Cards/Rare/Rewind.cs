using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Rewind : HexaghostCardModel
{
    public Rewind() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCards(1, 1);
        WithKeyword(HexaghostKeyword.Retract);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = await DownfallCardCmd.SelectFromCards(ctx, Owner.GetDiscard(),
            DownfallCardSelectorPrefs.ToHandSelectionPrompt, this, true);
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class Displace : HexaghostCardModel
{
    public Displace() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(7, 1);
        WithCards(1, 1);
        WithVar(new CardsVar("Place", 1));
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Draw(this, ctx);
        var cards = await DownfallCardCmd.SelectFromHand(ctx, 
            DownfallCardSelectorPrefs.ToTopSelectionPrompt,  DynamicVars["Place"].IntValue, this);
        await CardPileCmd.Add(cards, PileType.Draw, CardPilePosition.Top);
    }
}
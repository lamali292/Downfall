using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class SeventhEye : HexaghostCardModel
{
    public SeventhEye() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 3);
        WithKeyword(CardKeyword.Retain);
        WithVar("Discard", 1);
    }

    protected override Artist Artist => Artist.Get<Claude27A>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars["Discard"].IntValue);
        var selected = (await CardSelectCmd.FromHandForDiscard(ctx, Owner, prefs, null, this)).ToList();
        await CardCmd.Discard(ctx, selected);
    }
}
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Piles;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class ByrdsEye : AwakenedCardModel
{
    public ByrdsEye() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(AwakenedTip.Conjure);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded) AwakenedCmd.GetSpellbook(Owner)?.Refresh(Owner);
        var cards = AwakenedPile.Spellbook.GetPile(Owner).Cards;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = (await CardSelectCmd.FromSimpleGrid(ctx, cards, Owner, prefs)).FirstOrDefault();
        if (selected == null) return;
        await AwakenedCmd.ConjureSelected(Owner, this, selected);
    }
}
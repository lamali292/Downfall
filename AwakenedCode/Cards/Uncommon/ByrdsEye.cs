using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Piles;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
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
        WithConjure();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded) AwakenedCmd.GetSpellbook(Owner)?.Refresh(Owner);
        var cards = AwakenedPile.Spellbook.GetPile(Owner).Cards;
        var selected = (await DownfallCardCmd.SelectFromCards(ctx, cards, DownfallCardSelectorPrefs.ConjureSelectionPrompt, this)).FirstOrDefault();
        if (selected == null) return;
        await AwakenedCmd.ConjureSelected(Owner, this, selected);
    }
}
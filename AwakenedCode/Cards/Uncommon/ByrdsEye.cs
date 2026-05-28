using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Piles;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class ByrdsEye : AwakenedCardModel
{
    public ByrdsEye() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithConjure();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded) AwakenedCmd.GetSpellbook(Owner)?.Refresh(Owner);
        var cards = AwakenedPile.Spellbook.GetPile(Owner).Cards;
        var selected =
            (await DownfallCardCmd.SelectFromCards(ctx, cards, DownfallCardSelectorPrefs.ConjureSelectionPrompt, this))
            .FirstOrDefault();
        if (selected == null) return;
        await AwakenedCmd.ConjureSelected(Owner, this, selected);
    }
}
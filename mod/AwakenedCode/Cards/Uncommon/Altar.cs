using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Altar : AwakenedCardModel
{
    public Altar() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 3);
        WithTip(CardKeyword.Exhaust);
        WithConjure();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var card = await CommonActions.SelectSingleCard(this, CardSelectorPrefs.ExhaustSelectionPrompt, ctx,
            PileType.Hand);
        if (card != null) await CardCmdCompatibility.Exhaust(ctx, card);
        await AwakenedCmd.Conjure(Owner);
    }
}
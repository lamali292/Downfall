using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
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
        this.WithConjure();
    }
    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.CardBlock(this, cardPlay);
        var card = await CommonActions.SelectSingleCard(this, CardSelectorPrefs.ExhaustSelectionPrompt, ctx,
            PileType.Hand);
        if (card != null) await CardCmd.Exhaust(ctx, card);
        await AwakenedCmd.Conjure(Owner, CombatState);
    }
}
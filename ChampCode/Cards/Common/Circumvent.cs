using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Circumvent : ChampCardModel
{
    public Circumvent() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithCards(2);
        this.WithDefensiveTip();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool ShouldGlowRedInternal => !Owner.ShouldDefensiveComboTrigger();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Draw(this, ctx);

        if (Owner.ShouldDefensiveComboTrigger()) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars.Cards.IntValue);
        var cards = await CardSelectCmd.FromHandForDiscard(ctx, Owner, prefs, null, this);
        await CardCmd.Discard(ctx, cards);
    }
}
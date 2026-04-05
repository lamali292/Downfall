using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class Circumvent : ChampCardModel
{
    public Circumvent() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithCards(2);
    }

    protected override bool ShouldGlowGoldInternal => Owner.IsInChampStance<DefensiveChampStance>();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Draw(this, ctx);
        if (Owner.ShouldDefensiveComboTrigger()) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars.Cards.IntValue);
        var cards = await CardSelectCmd.FromHandForDiscard(ctx, Owner, prefs, null, this);
        await CardCmd.Discard(ctx, cards);
    }
}
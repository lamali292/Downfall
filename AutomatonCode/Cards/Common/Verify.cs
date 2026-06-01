using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Verify : AutomatonCardModel
{
    public Verify() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithTip(AutomatonTip.Stash);
        WithBlock(6, 3);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var card = (await CardSelectCmd.FromCombatPile(ctx, PileType.Discard.GetPile(Owner),
            Owner,
            new CardSelectorPrefs(StashCmd.StashSelectionPrompt, 1))).FirstOrDefault();
        if (card == null) return;
        await StashCmd.Stash(card);
    }
}
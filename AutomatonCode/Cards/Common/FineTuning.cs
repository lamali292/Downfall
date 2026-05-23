using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class FineTuning : AutomatonCardModel
{
    public FineTuning() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip(AutomatonTip.Stash);
        WithTip(CardKeyword.Retain);
        WithCards(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = (await CardSelectCmd.FromHand(ctx, Owner,
            new CardSelectorPrefs(StashCmd.StashSelectionPrompt, DynamicVars.Cards.IntValue), null,
            this)).ToList();
        // is order important?
        foreach (var card in cards)
        {
            CardCmd.Upgrade(card);
            card.AddKeyword(CardKeyword.Retain);
        }

        await StashCmd.Stash(cards);
    }
}
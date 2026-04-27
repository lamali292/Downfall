using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Displays;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class FineTuning : AutomatonCardModel
{
    public FineTuning() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequence = AutomatonCmd.GetSequence(Owner);
        foreach (var card in sequence)
        {
            foreach (var dynVar in card.DynamicVars.Values) dynVar.UpgradeValueBy(1m);
        }

        AutomatonDisplay.Refresh(Owner);
        return Task.CompletedTask;
    }
}
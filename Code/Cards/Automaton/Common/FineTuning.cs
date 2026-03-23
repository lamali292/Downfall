using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class FineTuning() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequence = AutomatonCmd.GetSequence(Owner.Creature);
        foreach (var card in sequence)
        {
            foreach (var dynVar in card.DynamicVars.Values) dynVar.UpgradeValueBy(1m);
        }

        AutomatonCmd.RefreshDisplay(Owner.Creature);
        return Task.CompletedTask;
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Automaton.Token;

[Pool(typeof(TokenCardPool))]
public class Debug : AutomatonCardModel
{
    public Debug() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequence = AutomatonCmd.GetSequence(Owner)
            .OfType<AutomatonCardModel>()
            .Where(c => c is ICompilableError)
            .ToList();

        foreach (var card in sequence)
            card.SuppressCompileError = true;
        AutomatonDisplay.Refresh(Owner);
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(0);
    }
}
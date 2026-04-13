using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Automaton.Token;

[Pool(typeof(TokenCardPool))]
public class Debug() : AutomatonCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var creature = cardPlay.Card.Owner.Creature;
        var sequence = AutomatonCmd.GetSequence(creature)
            .OfType<AutomatonCardModel>()
            .Where(c => c is ICompilableError)
            .ToList();

        foreach (var card in sequence)
            card.SuppressCompileError = true;
        AutomatonCmd.RefreshDisplay(creature);
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(0);
    }
}
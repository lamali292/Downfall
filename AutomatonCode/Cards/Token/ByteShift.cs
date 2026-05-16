using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class ByteShift : AutomatonCardModel
{
    public ByteShift() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(CardKeyword.Retain);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequencePile = AutomatonPile.FunctionSequence.GetPile(Owner);
        var choices = sequencePile.Cards.ToList();
        if (choices.Count == 0) return;

        foreach (var cardModel in choices) cardModel.AddKeyword(CardKeyword.Retain);

        await AutomatonCmd.MoveFromSequenceToHand(choices, Owner);
    }
}
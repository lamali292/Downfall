using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Piles;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class BitShift : AutomatonCardModel
{
    public BitShift() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(CardKeyword.Retain);
        WithTip(AutomatonTip.Encode);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequencePile = AutomatonPile.FunctionSequence.GetPile(Owner);
        var choices = sequencePile.Cards.ToList();
        if (choices.Count == 0) return;

        var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, Owner);
        if (card == null) return;
        card.AddKeyword(CardKeyword.Retain);
        await AutomatonCmd.MoveFromSequenceToHand(card, Owner);
    }
}
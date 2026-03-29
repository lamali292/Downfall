using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class BitShift() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequencePile = AutomatonPile.FunctionSequence.GetPile(Owner);
        var choices = sequencePile.Cards.ToList();
        if (choices.Count == 0) return;

        var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, Owner);
        if (card == null) return;

        await AutomatonCmd.MoveFromSequenceToHand(card, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
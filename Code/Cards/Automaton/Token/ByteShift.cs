using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Piles;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Automaton.Token;

[Pool(typeof(TokenCardPool))]
public class ByteShift() : AutomatonCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sequencePile = AutomatonPile.FunctionSequence.GetPile(Owner);
        var choices = sequencePile.Cards.ToList();
        if (choices.Count == 0) return;

        foreach (var cardModel in choices) cardModel.AddKeyword(CardKeyword.Retain);

        await AutomatonCmd.MoveFromSequenceToHand(choices, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
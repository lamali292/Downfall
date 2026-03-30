using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Automaton.Token;

[Pool(typeof(TokenCardPool))]
public class Decompile() : AutomatonCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        var sequence = AutomatonCmd.GetSequence(player)
            .OfType<AutomatonCardModel>()
            .ToList();
        var count = sequence.Count;
        foreach (var card in sequence)
            await CardCmd.Exhaust(ctx, card);


        await PlayerCmd.GainEnergy(count, cardPlay.Card.Owner);
        await CardPileCmd.Draw(ctx, count, cardPlay.Card.Owner);
        AutomatonDisplay.Refresh(player);
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
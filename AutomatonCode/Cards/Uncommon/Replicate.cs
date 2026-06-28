using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Replicate() : AutomatonCardModel(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy),
    IEncodable<ReplicateEncode>
{
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var copiedCard = cardPlay.Card.CreateClone();
        var result = await CardPileCmd.AddGeneratedCardToCombat(copiedCard, PileType.Discard, Owner);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.4f);
    }
}
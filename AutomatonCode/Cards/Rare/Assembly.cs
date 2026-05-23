using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Assembly : AutomatonCardModel
{
    public Assembly() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(4, 2);
        WithTip(AutomatonTip.Encode);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = await CommonActions.Draw(this, ctx);
        foreach (var card in cards.OfType<IEncodable>().Where(e => e is { AutoEncode: true }))
        {
            await card.Encode(ctx, cardPlay);
        }
    }
}
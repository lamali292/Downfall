using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Envision : AwakenedCardModel
{
    public Envision() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4, 3);
        WithConjure();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.CardBlock(this, cardPlay);
        var card = await AwakenedCmd.Conjure(Owner, CombatState);
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }
}
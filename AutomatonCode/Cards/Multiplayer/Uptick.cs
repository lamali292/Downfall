using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Multiplayer;

[Pool(typeof(AutomatonCardPool))]
public class Uptick : AutomatonCardModel
{
    public Uptick() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithKeyword(CardKeyword.Exhaust);
        this.WithPower<DrawCardsNextTurnPower>(2, 1, false);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<DrawCardsNextTurnPower>(ctx, this, cardPlay);
    }
}
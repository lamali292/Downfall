using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Basic;

[Pool(typeof(AutomatonCardPool))]
public class Postpone : AutomatonCardModel
{
    public Postpone() : base(2, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(10, 4);
        WithStash(1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await StashCmd.StashFromHand(this, ctx);
    }
}
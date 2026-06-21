using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Ancient;

[Pool(typeof(AutomatonCardPool))]
public class Hook : AutomatonCardModel
{
    public Hook() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithBlock(10, 4);
        WithStash(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await StashCmd.StashFromDraw(this, ctx);
    }
}
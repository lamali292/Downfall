using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class MaxOutput : AutomatonCardModel
{
    public MaxOutput() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithCards(3, 1);
        WithTip(typeof(Error));
        WithPower<MaxOutputPower>(1, false);
        WithTip(AutomatonTip.Stash);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await CommonActions.ApplySelf<MaxOutputPower>(ctx, this);
    }
}
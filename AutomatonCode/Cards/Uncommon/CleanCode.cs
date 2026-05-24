using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class CleanCode : AutomatonCardModel
{
    public CleanCode() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<CleanCodePower>(1, 1, false);
        WithTip(AutomatonTip.Stash);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CleanCodePower>(ctx, this);
    }
}
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Recreate : AutomatonCardModel
{
    public Recreate() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<RecreatePower>(1, false);
        WithPower<RecreatePlusPower>(1, false);
        WithUpgradingCardTip<Fuel>();
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => IsUpgraded ? 
            CommonActions.ApplySelf<RecreatePlusPower>(ctx, this) : 
            CommonActions.ApplySelf<RecreatePower>(ctx, this);
}
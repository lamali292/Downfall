using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ClassDefault : AutomatonCardModel
{
    public ClassDefault() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(AutomatonTip.Encode);
        WithPower<ClassDefaultPower>(2, false);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ClassDefaultPower>(ctx, this);
    }
}
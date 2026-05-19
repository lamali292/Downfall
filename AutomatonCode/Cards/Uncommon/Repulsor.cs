using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Repulsor : AutomatonCardModel
{
    public Repulsor() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(DownfallTip.Status);
        WithTip(CardKeyword.Exhaust);
        WithPower<ExhaustStatusesPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ExhaustStatusesPower>(ctx, this);
    }
}
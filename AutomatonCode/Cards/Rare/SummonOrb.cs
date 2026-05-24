using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SummonOrb : AutomatonCardModel
{
    public SummonOrb() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<SummonOrbPower>(1, false);
        WithTip(AutomatonTip.Stash);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SummonOrbPower>(ctx, this);
    }
}
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Crashout : AutomatonCardModel
{
    public Crashout() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<CrashoutPower>(10, 5);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<CrashoutPower>(ctx, this);
    }
}
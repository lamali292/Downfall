using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SentientForm : AutomatonCardModel
{
    public SentientForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<ArsenalPower>(2, 1, false);
        WithTip(typeof(StrengthPower));
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
     => CommonActions.ApplySelf<ArsenalPower>(ctx, this);
}
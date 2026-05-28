using Automaton.AutomatonCode.Core;
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
        this.WithPower<ArsenalPower>(2, 1, false);
        this.WithTip<StrengthPower>();
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<ArsenalPower>(ctx, this);
    }
}
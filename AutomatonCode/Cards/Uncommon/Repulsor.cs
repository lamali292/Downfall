using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Repulsor : AutomatonCardModel
{
    public Repulsor() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<RepulsePower>(4, 1, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<RepulsePower>(ctx, this);
    }
}
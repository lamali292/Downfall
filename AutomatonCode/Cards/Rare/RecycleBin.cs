using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class RecycleBin : AutomatonCardModel
{
    public RecycleBin() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<RecycleBinPower>(4, 1, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<RecycleBinPower>(ctx, this);
    }
}
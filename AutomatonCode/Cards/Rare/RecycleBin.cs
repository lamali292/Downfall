using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class RecycleBin : AutomatonCardModel
{
    public RecycleBin() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<RecycleBinPower>(4, 1, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<RecycleBinPower>(ctx, this);
    }
}
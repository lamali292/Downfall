using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Downfall;

public abstract class TemporaryPower<T> : DownfallPowerModel, ITemporaryPower
    where T : PowerModel
{
    private bool _shouldIgnoreNextInstance;

    public override PowerType Type => !IsPositive ? PowerType.Debuff : PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected virtual bool IsPositive => true;
    private int Sign => !IsPositive ? -1 : 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<T>()
    ];

    public abstract AbstractModel OriginModel { get; }
    public PowerModel InternallyAppliedPower => ModelDb.Power<T>();

    public void IgnoreNextInstance()
    {
        _shouldIgnoreNextInstance = true;
    }

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
            return;
        }

        await PowerCmd.Apply<T>(target, Sign * amount, applier, cardSource, true);
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (amount == Amount || power != this)
            return;

        if (_shouldIgnoreNextInstance)
            _shouldIgnoreNextInstance = false;
        else
            await PowerCmd.Apply<T>(Owner, Sign * amount, applier, cardSource, true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;

        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<T>(Owner, -Sign * Amount, Owner, null);
    }
}
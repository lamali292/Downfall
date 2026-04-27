using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Powers.Abstract;

public abstract class TemporaryPower<T> : DownfallPowerModel, ITemporaryPower
    where T : PowerModel
{
    private bool _shouldIgnoreNextInstance;

    public override PowerType Type => !IsPositive ? PowerType.Debuff : PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected virtual bool IsPositive => true;
    private int Sign => !IsPositive ? -1 : 1;
    protected virtual bool RemovedAfterOwnTurn => true;

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

        await PowerCmd.Apply<T>(new ThrowingPlayerChoiceContext(), target, Sign * amount, applier, cardSource, true);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount == Amount || power != this)
            return;

        if (_shouldIgnoreNextInstance)
            _shouldIgnoreNextInstance = false;
        else
            await PowerCmd.Apply<T>(ctx, Owner, Sign * amount, applier, cardSource, true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side != Owner.Side == RemovedAfterOwnTurn)
            return;

        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<T>(ctx, Owner, -Sign * Amount, Owner, null);
    }
}
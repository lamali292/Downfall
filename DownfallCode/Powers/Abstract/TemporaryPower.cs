using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Powers.Abstract;

public abstract class TemporaryPowerBase<TP> : DownfallPowerModel, ITemporaryPower
    where TP : PowerModel
{
    private bool _shouldIgnoreNextInstance;

    protected TemporaryPowerBase()
    {
        WithTip<TP>();
    }

    public override PowerType Type => !IsPositive ? PowerType.Debuff : PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected virtual bool IsPositive => true;
    private int Sign => !IsPositive ? -1 : 1;
    protected virtual bool RemovedAfterOwnTurn => true;


    public abstract AbstractModel OriginModel { get; }
    public PowerModel InternallyAppliedPower => ModelDb.Power<TP>();

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

        await PowerCmd.Apply<TP>(new ThrowingPlayerChoiceContext(), target, Sign * amount, applier, cardSource, true);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (amount == Amount || power != this) return;
        if (_shouldIgnoreNextInstance) _shouldIgnoreNextInstance = false;
        else await PowerCmd.Apply<TP>(ctx, Owner, Sign * amount, applier, cardSource, true);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side == RemovedAfterOwnTurn) return;
        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<TP>(ctx, Owner, -Sign * Amount, Owner, null);
    }
}

public abstract class TemporaryPower<TP> : TemporaryPowerBase<TP>
    where TP : PowerModel;

public abstract class TemporaryPower<TP, TC> : TemporaryPowerBase<TP>
    where TP : PowerModel
    where TC : DownfallCharacterModel
{
    public override string CustomPackedIconPath => $"{IconName}.tres".PowerImagePath<TC>();
    public override string CustomBigIconPath => $"{IconName}.png".BigPowerImagePath<TC>();
}
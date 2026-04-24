using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Guardian;

public abstract class GuardianModeModel : AbstractModel
{
    private Player? _player;

    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    protected ICombatState CombatState => Owner.Creature.CombatState ??
                                         throw new InvalidOperationException("Combat state not initialized");

    public GuardianModeModel ToMutable(Player player)
    {
        var mutable = (GuardianModeModel)MutableClone();
        mutable._player = player;
        return mutable;
    }


    public Task OnEnter()
    {
        return Task.CompletedTask;
    }

    public Task OnExit()
    {
        return Task.CompletedTask;
    }
}
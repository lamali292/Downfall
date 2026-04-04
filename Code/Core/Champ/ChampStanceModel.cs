using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Champ;

public abstract class StanceModel : AbstractModel
{
    public StanceModel ToMutable(Player player)
    {
        var mutable = (StanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }
    private Player? _player;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    public virtual Task OnEnter(PlayerChoiceContext ctx) => Task.CompletedTask;
    public virtual Task OnExit(PlayerChoiceContext ctx)  => Task.CompletedTask;
    public virtual Task SkillBonus(PlayerChoiceContext ctx)  => Task.CompletedTask;
}







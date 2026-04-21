using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Champ;

public abstract class ChampStanceModel : AbstractModel
{
    private Player? _player;

    public abstract bool HasFinisher { get; }
    public virtual string? ChargeIconPath => null;

    public int Charges { get; private set; }
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    protected CombatState CombatState => Owner.Creature.CombatState ??
                                         throw new InvalidOperationException("Combat state not initialized");

    public ChampStanceModel ToMutable(Player player)
    {
        var mutable = (ChampStanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    public void ResetCharges()
    {
        Charges = 3;
        ChampModel.RefreshDisplay(Owner);
    }

    public Task OnEnter(PlayerChoiceContext ctx)
    {
        ResetCharges();
        return Task.CompletedTask;
    }

    public Task OnExit(PlayerChoiceContext ctx)
    {
        Charges = 0;
        return Task.CompletedTask;
    }


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Owner != cardPlay.Card.Owner || cardPlay.Card.Type != CardType.Skill ||
            Owner.Creature.CombatState == null) return;

        if (!DownfallHook.IgnoreChargeCap(Owner.Creature.CombatState, Owner))
        {
            if (Charges <= 0) return;
            Charges--;
        }

        ChampModel.RefreshDisplay(Owner);
        await SkillBonus();
    }

    public virtual Task SkillBonus()
    {
        return Task.CompletedTask;
    }

    public virtual Task Finisher(PlayerChoiceContext ctx)
    {
        return Task.CompletedTask;
    }
}
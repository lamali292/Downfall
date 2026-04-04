using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Champ;

public abstract class ChampStanceModel : AbstractModel
{

    public abstract bool HasFinisher { get; }
    public virtual string? ChargeIconPath => null;
    public ChampStanceModel ToMutable(Player player)
    {
        var mutable = (ChampStanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    public int Charges { get; private set; }

    private Player? _player;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    public Task OnEnter(PlayerChoiceContext ctx)
    {
        Charges = 3;
        return Task.CompletedTask;
    }

    public Task OnExit(PlayerChoiceContext ctx)
    {
        Charges = 0;
        return Task.CompletedTask;
    }


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Owner != cardPlay.Card.Owner || cardPlay.Card.Type != CardType.Skill || Charges <= 0) return;
        Charges--;
        ChampModel.RefreshDisplay(Owner);
        await SkillBonus();
    }
    
    public virtual Task SkillBonus()  => Task.CompletedTask;
    public virtual Task Finisher(PlayerChoiceContext ctx)  => Task.CompletedTask;
}







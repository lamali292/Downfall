using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KinPriestCard : Collectible<TheKinBoss>
{
    public KinPriestCard() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithDamage(15, 5);
        WithPower<WeakPower>(3);
        WithPower<VulnerablePower>(3);
    }

    private bool EvenTurn => (Owner.PlayerCombatState?.TurnNumber ?? 0) % 2 == 0;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (EvenTurn)
        {
            await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        }
        else
        {
            await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("EvenTurn", EvenTurn);
    }
}
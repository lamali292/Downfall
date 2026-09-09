using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class AeonglassCard : Collectible<AeonglassBoss>
{
    public AeonglassCard() : base(12, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithVar("Mult", 3, 6);
        //Todo: Not entirely sure how gold axe works.
    }

    public override Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        ReduceCostBy(1);
        return Task.CompletedTask;
    }
    public void ReduceCostBy(int amount) => EnergyCost.AddThisCombat(-amount);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        //Todo: Implement card.
    }
}
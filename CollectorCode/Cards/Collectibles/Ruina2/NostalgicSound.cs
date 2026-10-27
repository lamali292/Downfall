using BaseLib.Utils;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class NostalgicSound : Ruina2Card
{
    public NostalgicSound() : base(9, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, "BLUE_STAR_ELITE")
    {
        WithCostUpgradeBy(-2);
        WithDamage(33);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
    
    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        if (card == this)
        {
            if (card.Owner.PlayerCombatState != null && card.Owner.PlayerCombatState.Reserve > 0)
            {
                modifiedCost = originalCost - card.Owner.PlayerCombatState.Reserve;
                return true;
            }   
        }
        modifiedCost = originalCost;
        return false;
    }
}
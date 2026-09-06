using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Collectibles;

public class SoulFyshCard : Collectible<SoulFyshBoss>
{
    public SoulFyshCard() : base(7, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithTip(CardKeyword.Unplayable);
        WithHpLoss(50, 16);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this) return false;
        modifiedCost -= Owner.Hand.Count(e => e.Keywords.Contains(CardKeyword.Unplayable));
        return true;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CompatibilityCreatureCmd.Damage(ctx, cardPlay.Target, DynamicVars.HpLoss.BaseValue,
            DamageProps.cardHpLoss, Owner.Creature, this, cardPlay);
    }
}

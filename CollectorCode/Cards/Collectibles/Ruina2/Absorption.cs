using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class Absorption : Ruina2Card
{
    public Absorption() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, "MOUNTAIN_ELITE", 0.56f)
    {
        WithDamage(8, 3);
        WithHeal(4, 2);
        WithKeyword(CollectorKeyword.Pyre);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (await PyreCmd.Pyre(ctx, this) == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.IntValue);
    }
}
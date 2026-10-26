using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class Wrath : Ruina2Card
{
    public Wrath() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, "WRATH_ELITE")
    {
        WithDamage(8, 2);
        WithRepeat(2);
        WithVar("Increase", 4, 1);
    }

    public Decimal ExtraDamageFromPlays
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        DamageVar damage = DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue + DynamicVars["Increase"].BaseValue;
        ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DamageVar damage = DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue + ExtraDamageFromPlays;
    }
}
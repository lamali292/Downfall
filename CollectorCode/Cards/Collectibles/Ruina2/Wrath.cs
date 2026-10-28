using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class Wrath : Ruina2Card
{
    public Wrath() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, "WRATH_ELITE", 0.45f)
    {
        WithDamage(8, 2);
        WithRepeat(2);
        WithVar("Increase", 4, 1);
    }

    private decimal ExtraDamageFromPlays
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
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
        DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
        ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
    }
}
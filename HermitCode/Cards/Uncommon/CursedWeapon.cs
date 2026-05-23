using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class CursedWeapon : HermitCardModel
{
    private const string IncreaseKey = "Increase";
    private const int BaseDamage = 10;
    private int _currentDamage = 10;
    private int _increasedDamage;

    public CursedWeapon() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithCostUpgradeBy(-1);
        WithDamage(CurrentDamage);
        WithKeyword(CardKeyword.Exhaust);
        WithHpLoss(2);
        WithVar(IncreaseKey, 1);
    }

    [SavedProperty]
    private int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    private int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.Damage(ctx, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitFireHitFx()
            .Execute(ctx);

        var increase = DynamicVars[IncreaseKey].IntValue;
        Owner.GetAllCards().OfType<CursedWeapon>().ToList().ForEach(card =>
        {
            card.BuffFromPlay(increase);
            (card.DeckVersion as CursedWeapon)?.BuffFromPlay(increase);
        });
    }

    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = BaseDamage + IncreasedDamage;
    }
}
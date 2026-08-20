using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
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
        this.WithHpLoss(2);
        WithVar(IncreaseKey, 1);
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public override void AfterCreated()
    {
        base.AfterCreated();

        // Future cursed weapons join at the current shared level
        var others = GetCursedWeapons().Where(c => c != this).ToList();
        if (others.Count > 0)
            IncreasedDamage = others.Max(c => c.IncreasedDamage);

        UpdateDamage();
    }
    
    private void SetIncrease(int total)
    {
        IncreasedDamage = total;
        UpdateDamage();
    }
    
    private List<CursedWeapon> GetCursedWeapons()
    {
        return Owner.GetAllCombatCards.OfType<CursedWeapon>()
            .Concat(Owner.DeckPile.OfType<CursedWeapon>())
            .Distinct()
            .ToList();
    }
    
    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public int CurrentDamage
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
    // ReSharper disable once MemberCanBePrivate.Global
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }


protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
{
    await CompatibilityCreatureCmd.Damage(ctx, Owner.Creature, DynamicVars.HpLoss.BaseValue,
        DamageProps.cardHpLoss, Owner.Creature, this, play);

    await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
    await CommonActions.CardAttack(this, play).WithHermitFireHitFx()
        .Execute(ctx);

    var increase = DynamicVars[IncreaseKey].IntValue;

    // One shared total, applied to every cursed weapon (deck + combat copies)
    var weapons = GetCursedWeapons();
    var newTotal = weapons.Select(c => c.IncreasedDamage).DefaultIfEmpty(0).Max() + increase;
    weapons.ForEach(c => c.SetIncrease(newTotal));
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
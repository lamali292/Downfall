using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class GoldenBullet : HermitCardModel
{
    private int _currentCost = 3;

    public GoldenBullet() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(18, 6);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(StaticHoverTip.Fatal);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    [SavedProperty]
    public int CurrentCost
    {
        get => _currentCost;
        set
        {
            AssertMutable();
            _currentCost = value;
            EnergyCost.SetCustomBaseCost(_currentCost);
        }
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        var shouldTriggerFatal = play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await CommonActions.CardAttack(this, play).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun1();
                return Task.CompletedTask;
            })
            .Execute(ctx);

        if (shouldTriggerFatal && attackCommand.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
        {
            BuffFromPlay();
            (DeckVersion as GoldenBullet)?.BuffFromPlay();
        }
    }

    protected override void AfterDowngraded()
    {
        UpdateCost();
    }

    private void BuffFromPlay()
    {
        CurrentCost = Math.Max(0, CurrentCost - 1);
        UpdateCost();
    }

    private void UpdateCost()
    {
        EnergyCost.SetCustomBaseCost(CurrentCost);
    }
}
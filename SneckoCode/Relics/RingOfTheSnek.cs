using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class RingOfTheSnek : SneckoRelicModel, IAfterOverflowEffect
{
    private bool _isActivating;
    private int _overflowEffectsPlayed;

    public RingOfTheSnek() : base(RelicRarity.Rare)
    {
        WithVars(
            new PowerVar<WeakPower>(1),
            new PowerVar<VulnerablePower>(1),
            new CardsVar(3)
        );
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount =>
        !IsActivating ? OverflowEffectsPlayed % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;


    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    private int OverflowEffectsPlayed
    {
        get => _overflowEffectsPlayed;
        set
        {
            AssertMutable();
            _overflowEffectsPlayed = value;
            UpdateDisplay();
        }
    }

    public async Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            Owner.Creature.CombatState == null) return;
        OverflowEffectsPlayed++;
        var intValue = DynamicVars.Cards.IntValue;
        if (OverflowEffectsPlayed % intValue != 0) return;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        var target = Owner.RunState.Rng.CombatTargets.NextItem(Owner.Creature.CombatState.HittableEnemies);
        if (target == null) return;
        await PowerCmd.Apply<WeakPower>(ctx, target, DynamicVars.Power<WeakPower>().BaseValue, Owner.Creature, null);
        await PowerCmd.Apply<VulnerablePower>(ctx, target, DynamicVars.Power<VulnerablePower>().BaseValue,
            Owner.Creature, null);
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            var intValue = DynamicVars.Cards.IntValue;
            Status = OverflowEffectsPlayed % intValue == intValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task BeforeCombatStart()
    {
        OverflowEffectsPlayed = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        IsActivating = false;
        return Task.CompletedTask;
    }
}
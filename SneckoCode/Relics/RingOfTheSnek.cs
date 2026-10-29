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
using MegaCrit.Sts2.Core.Saves.Runs;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class RingOfTheSnek : SneckoRelicModel, IAfterOverflowEffect
{
    public RingOfTheSnek() : base(RelicRarity.Rare)
    {
        WithVars(
            new PowerVar<WeakPower>(1),
            new PowerVar<VulnerablePower>(1),
            new CardsVar(3)
        );
        WithTip(SneckoKeywords.Overflow);
    }

    public override bool ShowCounter => true;

    public override int DisplayAmount =>
        !IsActivating ? OverflowEffectsPlayed : DynamicVars.Cards.IntValue;


    private bool IsActivating
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateDisplay();
        }
    }

    // Carries over between combats and through save/load, like the relic counter in Downfall 1.
    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public int OverflowEffectsPlayed
    {
        get;
        private set
        {
            AssertMutable();
            field = value % DynamicVars.Cards.IntValue;
            UpdateDisplay();
        }
    }

    public async Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress ||
            Owner.Creature.CombatState == null) return;
        OverflowEffectsPlayed++;
        if (OverflowEffectsPlayed != 0) return;
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
            Status = OverflowEffectsPlayed == DynamicVars.Cards.IntValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}
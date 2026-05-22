using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Rooms;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class Zetsumei : AwakenedRelicModel
{
    private bool _isActivating;
    private int _spellsPlayed;

    public Zetsumei() : base(RelicRarity.Uncommon)
    {
        WithCards(4);
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount
        => !IsActivating ? SpellsPlayed % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;

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

    private int SpellsPlayed
    {
        get => _spellsPlayed;
        set
        {
            AssertMutable();
            _spellsPlayed = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            var required = DynamicVars.Cards.IntValue;
            Status = SpellsPlayed % required == required - 1
                ? RelicStatus.Active
                : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task BeforeCombatStart()
    {
        // Resets the count clean for the new encounter
        SpellsPlayed = 0;
        UpdateDisplay();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !CombatManager.Instance.IsInProgress || cardPlay.Card is not ISpell)
            return;

        SpellsPlayed++;

        if (SpellsPlayed % DynamicVars.Cards.IntValue != 0)
            return;

        _ = TaskHelper.RunSafely(DoActivateVisuals());
        await DownfallCardCmd.GiveCard<Ceremony>(Owner, PileType.Hand);
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
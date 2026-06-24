/*using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

public class FireAway : HermitCardModel, IAfterDeadOnTrigger
{
    public FireAway() : base(9, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(3);
        this.WithRepeat(6);
        WithCostUpgradeBy(-2);
        WithEnergy(2);
        WithTip(HermitKeywords.DeadOn);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).BeforeDamage(() =>
        {
            HermitSfx.PlayGun3();
            return Task.CompletedTask;
        }).Execute(ctx);
    }

    private int DeadOnCount => CombatManager.Instance.History.Entries.OfType<DeadOnEntry>()
        .Count(e => e.CardPlay.Card.Owner == Owner);


    public override Task AfterCardEnteredCombat(CardModel card)
    {
        
        if (card != this || IsClone)
            return Task.CompletedTask;
        EnergyCost.AddThisCombat(-DeadOnCount * DynamicVars.Energy.IntValue);
        return Task.CompletedTask;
    }
    
    public Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;
        EnergyCost.AddThisCombat(-DynamicVars.Energy.IntValue);
        return Task.CompletedTask;
    }
}*/
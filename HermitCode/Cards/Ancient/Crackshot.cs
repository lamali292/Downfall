using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Ancient;

public class Crackshot : HermitCardModel
{
    public Crackshot() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<MalleableFrog>();

    public override bool GainsBlock => true;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null || cardPlay.Target == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var unblockedDamage = 0;
        var context = await AttackCommand.CreateContextAsync(CombatState, ctx, cardPlay);
        var list1 = (await CreatureCmd.Damage(ctx, cardPlay.Target, DynamicVars.Damage.BaseValue,
            DynamicVars.Damage.Props, this, cardPlay)).ToList();
        unblockedDamage += list1.Sum(e => e.TotalDamage + e.OverkillDamage);
        context.AddHit(list1);
        HermitSfx.PlayGun1();
        
        await HermitCmd.DeadOn(ctx, this, cardPlay, async () =>
        {
            var list2 = (await CreatureCmd.Damage(ctx, cardPlay.Target, DynamicVars.Damage.BaseValue,
                DynamicVars.Damage.Props, this, cardPlay)).ToList();
            unblockedDamage += list2.Sum(e => e.TotalDamage + e.OverkillDamage);
            context.AddHit(list2);
            HermitSfx.PlayGun1();
        });
        await context.DisposeAsync();
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, BlockProps.card, cardPlay);
      
    }
}
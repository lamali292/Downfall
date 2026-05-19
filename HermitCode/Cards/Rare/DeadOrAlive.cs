using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class DeadOrAlive : HermitCardModel
{

    private const int MonsterGoldAmount = 15;
    private const int EliteGoldAmount = 40;
    private const int BossGoldAmount = 100;

    public DeadOrAlive() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(HermitKeywords.Bounty);
        WithTip(StaticHoverTip.Fatal);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        var times = EnergyCost.CapturedXValue;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var command = await CommonActions.CardAttack(this, play, times).WithHermitBluntLightHitFx()
            .Execute(ctx);
        var shouldTriggerFatal = play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var wasTargetKilled = command.Results.SelectMany(e => e).Any(e => e.WasTargetKilled);
        if (!wasTargetKilled || !shouldTriggerFatal) return;
        var currentRoom = Owner.Creature.CombatState?.RunState.CurrentRoom;
        var goldAmount = currentRoom?.RoomType switch
        {
            RoomType.Monster => MonsterGoldAmount,
            RoomType.Elite => EliteGoldAmount,
            RoomType.Boss => BossGoldAmount,
            _ => MonsterGoldAmount
        };
        await PlayerCmd.GainGold(goldAmount, Owner);
        
    }
}
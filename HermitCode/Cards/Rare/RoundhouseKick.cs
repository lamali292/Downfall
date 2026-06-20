using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class RoundhouseKick : HermitCardModel
{
    public RoundhouseKick() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(13, 5);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(StaticHoverTip.Stun);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var attack = await CommonActions.CardAttack(this, play)
            .WithHermitBluntHeavyHitFx()
            .Execute(ctx);

        var hitEnemies = attack.Results.SelectMany(e => e).Select(e => e.Receiver).Distinct();
        foreach (var enemy in hitEnemies)
        {
            if (enemy.IsDead) continue;
            var monster = enemy.Monster;
            if (monster == null) continue;
            if (!monster.IntendsToAttack) await CreatureCmd.Stun(enemy);
        }
    }
}
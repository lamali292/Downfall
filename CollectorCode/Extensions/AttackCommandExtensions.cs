using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Extensions;

public static class AttackCommandExtensions
{
    extension(AttackCommand command)
    {
        public AttackCommand FromTorchhead(TorchheadMonsterModel slime, CardModel? card, CardPlay? cardPlay)
        {
            command.Attacker = command.Attacker == null
                ? slime.Creature
                : throw new InvalidOperationException("Attacker has already been set.");
            command.ModelSource = card;
            command.SetCardPlayCompat(cardPlay);
            command._attackerAnimName = "Attack";
            command._attackerAnimDelay = 0.3f;
            command._sourceType = AttackCommand.SourceType.Card;
            return command;
        }
    }
    
    public static async Task<AttackCommand?> ExecuteIfPresent(
        this AttackCommand? attack, PlayerChoiceContext ctx)
        => attack != null ? await attack.Execute(ctx) : null;
}
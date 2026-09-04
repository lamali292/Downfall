using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands.Builders;
using SlimeBoss.SlimeBossCode.Slimes;

namespace Collector.CollectorCode.Extensions;

public static class AttackCommandExtensions
{
    extension(AttackCommand command)
    {
        public AttackCommand FromTorchhead(TorchheadMonsterModel slime)
        {
            command.Attacker = command.Attacker == null
                ? slime.Creature
                : throw new InvalidOperationException("Attacker has already been set.");
            command._attackerAnimName = "Attack";
            command._sourceType = AttackCommand.SourceType.None;
            return command;
        }
    }
}
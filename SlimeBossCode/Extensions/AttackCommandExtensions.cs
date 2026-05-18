using MegaCrit.Sts2.Core.Commands.Builders;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Extensions;

public static class AttackCommandExtensions
{
    public static AttackCommand FromSlime(this AttackCommand command, SlimeModel slime)
    {
        command.Attacker = command.Attacker == null
            ? slime.Creature
            : throw new InvalidOperationException("Attacker has already been set.");
        command._attackerAnimName = "Attack";
        command._sourceType = AttackCommand.SourceType.None;
        return command;
    }
}
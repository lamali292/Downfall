using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Extensions;

internal static class ValuePropExtensions
{
    public static bool IsPoweredAttack(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
    }

    public static bool IsPoweredCardOrMonsterMoveBlock(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
    }

    public static bool IsCardOrMonsterMove(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move);
    }
}
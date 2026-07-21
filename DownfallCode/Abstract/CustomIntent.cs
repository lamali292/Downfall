using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Downfall.DownfallCode.Abstract;

public abstract class CustomIntent : AbstractIntent, ICustomModel
{
    protected override string IntentPrefix => GetType().GetPrefix() + GetType().Name.ToSnakeCase().ToUpperInvariant();
    protected override string? SpritePath => null;

    protected abstract string IntentSpritePath { get; }

    private void EnsureRegistered()
    {
        var key = IntentPrefix.ToLowerInvariant();
        if (IntentAnimData._data.ContainsKey(key)) return;
        IntentAnimData._data[key] = new IntentAnimData.InternalData
        {
            frames = [IntentSpritePath]
        };
    }

    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
    {
        EnsureRegistered();
        return base.GetAnimation(targets, owner);
    }
}
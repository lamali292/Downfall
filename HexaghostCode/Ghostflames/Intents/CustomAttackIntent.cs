using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames.Intents;

public class CustomAttackIntent(Func<int> damage, Func<int> repeat) : CustomIntent
{
    public override IntentType IntentType => IntentType.Attack;

    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
        => damage() switch
        {
            < 5  => IntentAnimData.attack1,
            < 10 => IntentAnimData.attack2,
            < 20 => IntentAnimData.attack3,
            < 40 => IntentAnimData.attack4,
            _    => IntentAnimData.attack5
        };

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = new LocString("intents", "FORMAT_DAMAGE_MULTI");
        label.Add("Damage", damage());
        label.Add("Repeat", repeat());
        return label;
    }
}
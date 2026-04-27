using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames.Intents;

public class CustomAttackIntent(Func<int> damage, Func<int> repeat) : CustomIntent
{
    public override IntentType IntentType => IntentType.Attack;
    protected override string IntentSpritePath => GetTieredSpritePath(damage());

    private static string GetTieredAnimation(int damage) => damage switch
    {
        < 5 => IntentAnimData.attack1,
        < 10 => IntentAnimData.attack2,
        < 20 => IntentAnimData.attack3,
        < 40 => IntentAnimData.attack4,
        _ => IntentAnimData.attack5
    };

    private static string GetTieredSpritePath(int damage) => damage switch
    {
        < 5 => "atlases/intent_atlas.sprites/attack/intent_attack_1.tres",
        < 10 => "atlases/intent_atlas.sprites/attack/intent_attack_2.tres",
        < 20 => "atlases/intent_atlas.sprites/attack/intent_attack_3.tres",
        < 40 => "atlases/intent_atlas.sprites/attack/intent_attack_4.tres",
        _ => "atlases/intent_atlas.sprites/attack/intent_attack_5.tres"
    };

    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
        => GetTieredAnimation(damage());

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = new LocString("intents", "FORMAT_DAMAGE_MULTI");
        label.Add("Damage", damage());
        label.Add("Repeat", repeat());
        return label;
    }
}
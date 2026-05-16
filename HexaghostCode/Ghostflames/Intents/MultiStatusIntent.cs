using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames.Intents;

public class MultiStatusIntent<T>(Func<int> amount, int repeat) : CustomIntent
    where T : PowerModel
{
    public override IntentType IntentType => IntentType.Debuff;
    protected override string IntentSpritePath => ModelDb.Power<T>().PackedIconPath;

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = new LocString("intents", "FORMAT_DAMAGE_MULTI");
        label.Add("Damage", amount());
        label.Add("Repeat", repeat);
        return label;
    }
}
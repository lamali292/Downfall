using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames.Intents;

public class BolsteringIntent(Func<int> block) : CustomIntent
{
    public override IntentType IntentType => IntentType.Defend;
    
    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentAnimData.defend;
    }
    
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = new LocString("intents", "FORMAT_DAMAGE_SINGLE");
        label.Add("Damage", block());
        return label;
    }
}
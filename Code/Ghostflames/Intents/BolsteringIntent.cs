using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Downfall.Code.Ghostflames.Intents;

public class BolsteringIntent : CustomIntent
{
    public override IntentType IntentType => IntentType.Debuff;
    protected override string IntentSpritePath => ModelDb.Power<StrengthPower>().PackedIconPath;
    
}
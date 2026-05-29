using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class EyeOfTheOccult : AwakenedRelicModel
{
    public EyeOfTheOccult() : base(RelicRarity.Event)
    {
        this.WithTip<Thunderbolt>();
        this.WithTip<Darkleech>();
    }
}
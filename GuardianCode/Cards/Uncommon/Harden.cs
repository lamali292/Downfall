using BaseLib.Utils;
using Downfall.DownfallCode.Powers;

using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Harden : GuardianCardModel
{
    public Harden() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<PlatedArmorPower>(4, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    }
}
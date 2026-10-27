using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Harden : GuardianCardModel
{
    public Harden() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<PlatedArmorPower>(3, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    }
}
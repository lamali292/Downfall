using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Harden : GuardianCardModel
{
    public Harden() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<MetallicizePower>(3, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MetallicizePower>(ctx, this);
    }
}
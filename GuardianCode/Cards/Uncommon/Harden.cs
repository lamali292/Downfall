using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Harden : GuardianCardModel, ITickCard
{
    public Harden() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<MetallicizePower>(3, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MetallicizePower>(ctx, this);
    }
}
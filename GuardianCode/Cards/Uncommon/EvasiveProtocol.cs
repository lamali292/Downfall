using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class EvasiveProtocol : GuardianCardModel
{
    public EvasiveProtocol() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithBrace(6, 3);
        this.WithPower<EvasiveProtocolPower>(1, 1, false);
        WithTip(GuardianTip.Polish);
        WithTip(GuardianTip.DefensiveMode);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EvasiveProtocolPower>(ctx, this);
        await GuardianCmd.Brace(ctx, this);
    }
}
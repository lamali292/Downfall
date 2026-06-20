using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class SpikerProtocol : GuardianCardModel
{
    public SpikerProtocol() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<SpikerProtocolPower>(2, 1, false);
        this.WithBrace(6, 3);
        this.WithTip<ThornsPower>();
        WithTip(GuardianTip.DefensiveMode);
    }

    protected override Artist Artist => Artist.Get<Ez>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpikerProtocolPower>(ctx, this);
        await GuardianCmd.Brace(ctx, this);
    }
}
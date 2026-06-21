using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class RevengeProtocol : GuardianCardModel
{
    public RevengeProtocol() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<BracingPower>(4, 2, false);
        this.WithPower<RevengeProtocolPower>(2, 1, false);
        this.WithTip<StrengthPower>();
        WithTip(GuardianTip.DefensiveMode);
        WithTip(GuardianTip.Brace);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RevengeProtocolPower>(ctx, this);
        await CommonActions.ApplySelf<BracingPower>(ctx, this);
    }
}
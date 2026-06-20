using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class SharpHide : GuardianCardModel
{
    public SharpHide() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<DexterityPower>(1);
        this.WithPower<SharpHidePower>(2, 2, false);
        this.WithTip<ThornsPower>();
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        await CommonActions.ApplySelf<SharpHidePower>(ctx, this);
    }
}
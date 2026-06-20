using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class BronzeBrambles : GuardianCardModel
{
    public BronzeBrambles() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        this.WithPower<BronzeBramblesPower>(1, false);
        this.WithTip<ThornsPower>();
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BronzeBramblesPower>(ctx, this);
    }
}
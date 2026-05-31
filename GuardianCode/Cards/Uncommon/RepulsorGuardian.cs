using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class RepulsorGuardian : GuardianCardModel
{
    public RepulsorGuardian() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(CardKeyword.Exhaust);
        this.WithPower<ExhaustStatusesPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ExhaustStatusesPower>(ctx, this);
    }
}
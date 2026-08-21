using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class GemFinder : GuardianCardModel
{
    public GemFinder() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<GemFinderPower>(1, false);
        WithTip(GuardianTip.Socket);
        WithTip(GuardianKeyword.Gem);
        WithCostUpgradeBy(-1);
    }
    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GemFinderPower>(ctx, this);
    }
}
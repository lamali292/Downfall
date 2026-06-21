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
    protected override bool HasEnergyCostX => true;


    public GemFinder() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        //this.WithPower<GemFinderPower>(1, false);
        WithTip(GuardianKeyword.Gem);
        WithTip(GuardianTip.Brace);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded) x++;
        await CommonActions.ApplySelf<GemFinderPower>(ctx, this, x);
    }
}
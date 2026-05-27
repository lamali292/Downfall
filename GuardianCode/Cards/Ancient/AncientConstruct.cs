using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Ancient;

[Pool(typeof(GuardianCardPool))]
public class AncientConstruct : GuardianCardModel
{
    public AncientConstruct() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithPower<ArtifactPower>(1);
        this.WithPower<AncientConstructPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ArtifactPower>(ctx, this);
        await CommonActions.ApplySelf<AncientConstructPower>(ctx, this);
    }
}
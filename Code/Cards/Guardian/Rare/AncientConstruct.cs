using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class AncientConstruct : GuardianCardModel
{
    public AncientConstruct() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<ArtifactPower>(1);
        WithPower<AncientConstructPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ArtifactPower>(ctx, this);
        await CommonActions.ApplySelf<AncientConstructPower>(ctx, this);
    }
}
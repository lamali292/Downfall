using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Daggerstorm : AwakenedCardModel
{
    public Daggerstorm() : base(2, CardType.Power, CardRarity.Token, TargetType.None)
    {
        WithPower<DaggerstormPower>(4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<DaggerstormPower>(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<DaggerstormPower>().UpgradeValueBy(2);
    }
    // TODO: Implement
}
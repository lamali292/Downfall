using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Rebirth : AwakenedCardModel
{
    public Rebirth() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<AwakeningPower>(8);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<AwakeningPower>(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<AwakeningPower>().UpgradeValueBy(3);
    }
}
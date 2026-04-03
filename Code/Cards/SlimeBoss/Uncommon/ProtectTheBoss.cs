using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.SlimeBoss.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class ProtectTheBoss : SlimeBossCardModel
{
    public ProtectTheBoss() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


    protected override void OnUpgrade()
    {
    }
}
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Doomsday : HexaghostCardModel
{
    public Doomsday() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<DoomsdayPower>(1);
        WithCostUpgradeBy(-1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DoomsdayPower>(ctx, this);
    }


}
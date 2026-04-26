using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class UnlimitedPower : HexaghostCardModel
{
    public UnlimitedPower() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await HexaghostCmd.IgniteAll(ctx, Owner);
    }


}
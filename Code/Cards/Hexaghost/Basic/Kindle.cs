using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Basic;

[Pool(typeof(HexaghostCardPool))]
public class Kindle : HexaghostCardModel
{
    public Kindle() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithKeyword(DownfallKeywords.Advance, UpgradeType.Add);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await HexaghostCmd.Ignite(ctx, Owner);
    }
}
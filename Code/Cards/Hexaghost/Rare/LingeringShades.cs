using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class LingeringShades : HexaghostCardModel
{
    public LingeringShades() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(DownfallKeywords.Retract);
        WithPower<SoulBurnPower>(14, 4);
    }
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


}
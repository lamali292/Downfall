using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class Spook : HexaghostCardModel
{
    public Spook() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(6, 3);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}
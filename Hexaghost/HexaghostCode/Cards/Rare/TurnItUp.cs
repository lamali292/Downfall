using BaseLib.Utils;
using Downfall.DownfallCode.Powers;

using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class TurnItUp : HexaghostCardModel
{
    public TurnItUp() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<TemporaryStrengthUpPower>(2, 1);
        WithPower<TemporaryDexterityUpPower>(2, 1);
        WithPower<TemporaryIntensityPower>(2, 1);
        WithKeyword(CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryDexterityUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryIntensityPower>(ctx, this);
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Incorporeal : HexaghostCardModel
{
    public Incorporeal() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithHpLoss(6, -3);
        WithPower<IntangiblePower>(1);
        WithKeywords(CardKeyword.Exhaust, HexaghostKeyword.Retract);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.LoseHp(ctx, this, cardPlay.Target);
        await CommonActions.ApplySelf<IntangiblePower>(ctx, this);
    }
}
using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class BacktrackSmack : HexaghostCardModel
{
    public BacktrackSmack() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(HexaghostKeyword.Retract);
        WithDamage(5, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2).Execute(ctx);
    }
}
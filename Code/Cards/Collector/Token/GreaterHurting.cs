using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Collector.Token;

[Pool(typeof(TokenCardPool))]
public class GreaterHurting : CollectorCardModel
{
    public GreaterHurting() : base(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(20, 6);
        WithTip(typeof(GreatestHurting));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        (await DownfallCardCmd.GiveCard<GreatestHurting>(Owner, PileType.Hand, upgraded: IsUpgraded))
            .GiveSingleTurnRetain();
    }
}
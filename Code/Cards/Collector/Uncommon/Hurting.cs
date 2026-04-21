using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Hurting : CollectorCardModel
{
    public Hurting() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(10, 3);
        WithTip(typeof(GreaterHurting));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        (await DownfallCardCmd.GiveCard<GreaterHurting>(Owner, PileType.Hand, upgraded: IsUpgraded))
            .GiveSingleTurnRetain();
    }
}
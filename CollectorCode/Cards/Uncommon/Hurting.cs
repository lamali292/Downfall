using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Hurting : CollectorCardModel
{
    public Hurting() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(10, 3);
        this.WithTip<GreaterHurting>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
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
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Whomp : CollectorCardModel
{
    public Whomp() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(14, 3);
        WithKindle(11, 3);
        WithUpgradeChangingCardTip<Burn, Ember>(null, (e, _) => CardCmd.Upgrade(e));
        //WithUpgradeChangingCardTip<Burn, Ember>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CollectorCmd.Kindle(ctx, this);
        if (IsUpgraded)
            await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand, upgraded: true);
        else 
            await DownfallCardCmd.GiveCard<Burn>(Owner, PileType.Hand);
    }
}
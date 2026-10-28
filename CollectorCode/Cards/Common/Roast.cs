using Collector.CollectorCode.Cards.Token;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Roast : CollectorCardModel
{
    public Roast() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithDamage(12, 4);
        WithUpgradeChangingCardTip<Burn, Ember>();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (await PyreCmd.Pyre(ctx, this) == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (IsUpgraded)
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Discard, 1);
        else
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Discard, 1);
    }
}
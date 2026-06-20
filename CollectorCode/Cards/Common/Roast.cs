using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Roast : CollectorCardModel, IHasPyre
{
    public Roast() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithDamage(4, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel? PyredCard { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
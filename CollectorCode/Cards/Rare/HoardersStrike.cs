using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class HoardersStrike : CollectorCardModel, IHasPyre, IShouldExhaustPyred
{
    public HoardersStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithDamage(16, 4);
        WithTags(CardTag.Strike);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel? PyredCard { get; set; }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = PyredCard;
        if (card == null || !card.VisualCardPool.IsColorless) return;
        card.ExhaustOnNextPlay = true;
        await CardCmd.AutoPlay(ctx, card, null);
    }
    
    public bool ShouldExhaustPyred(CardModel card, CardModel pyred)
    {
        return !(card == this && pyred.VisualCardPool.IsColorless);
    }
}
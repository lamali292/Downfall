using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Flash : CollectorCardModel, IHasPyre
{
    public Flash() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(new TooltipSource(c =>
        {
            var card = ModelDb.GetById<Trip>(ModelDb.Card<Trip>().Id).ToMutable();
            if (c.IsUpgraded) card.UpgradeInternal();
            return HoverTipFactory.FromCard(card);
        }));
        WithTip(new TooltipSource(c =>
        {
            var card = ModelDb.GetById<Blind>(ModelDb.Card<Blind>().Id).ToMutable();
            if (c.IsUpgraded) card.UpgradeInternal();
            return HoverTipFactory.FromCard(card);
        }));
    }

    public CardModel? PyredCard { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var trip = CombatState!.CreateCard<Trip>(cardPlay.Card.Owner);
        var blind = CombatState!.CreateCard<Blind>(cardPlay.Card.Owner);
        if (IsUpgraded)
        {
            trip.UpgradeInternal();
            blind.UpgradeInternal();
        }

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, [trip, blind], Owner);
        if (chosen == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner);
    }
}
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class Friend : Ruina2Card
{
    public Friend() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, "LAETITIA_ELITE", 0.4f)
    {
        WithKindle(11, 3);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.Kindle(ctx,  this);
        ModelId id = new ModelId("CARD", "RUINA2-GIFT");
        var card = ModelDb.GetByIdOrNull<CardModel>(id);
        if (card != null && CombatState != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard(card, Owner), PileType.Discard, Owner));
        }
    }
}
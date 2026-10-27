using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class Friend : Ruina2Card
{
    private static readonly ModelId GiftId = new("CARD", "RUINA2-GIFT");

    protected override MonsterModel? OverlayMonster => EncounterModel?.AllPossibleMonsters.LastOrDefault();
    public Friend() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, "LAETITIA_ELITE", 0.4f)
    {
        WithKindle(11, 3);
        WithKeyword(CardKeyword.Exhaust);
        WithTips(_ => ModelDb.GetByIdOrNull<CardModel>(GiftId) is { } gift ? [HoverTipFactory.FromCard(gift)] : []);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.Kindle(ctx,  this);
        var card = ModelDb.GetByIdOrNull<CardModel>(GiftId);
        if (card != null && CombatState != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard(card, Owner), PileType.Discard, Owner));
        }
    }
}
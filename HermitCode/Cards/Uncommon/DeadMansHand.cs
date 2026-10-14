using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class DeadMansHand : HermitCardModel
{
    private const int DrawCount = 3;

    public DeadMansHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(3);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    private static int RarityLevel(CardRarity rarity)
    {
        return rarity switch
        {
            CardRarity.Ancient => 10,
            CardRarity.Rare => 9,
            CardRarity.Uncommon => 8,
            CardRarity.Common => 7,
            CardRarity.Basic => 6,
            CardRarity.Token => 5,
            CardRarity.Curse => 4,
            CardRarity.Status => 3,
            CardRarity.Quest => 2,
            CardRarity.Event => 1,

            _ => 0
        };
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var handCards = Owner.Hand;
        if (handCards.Count > 0) await CardCmd.Discard(ctx, handCards);
        var rarest = Owner.DrawPile
            .OrderByDescending(c => RarityLevel(c.Rarity))
            .Take(3)
            .ToList();
        await CardPileCmd.Add(rarest, PileType.Hand);
    }
}
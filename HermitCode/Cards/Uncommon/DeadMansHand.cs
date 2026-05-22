using Downfall.DownfallCode.Extensions;
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

    private static int RarityLevel(CardRarity rarity)
    {
        return rarity switch
        {
            CardRarity.Ancient => 3,
            CardRarity.Rare => 2,
            CardRarity.Uncommon => 1,
            _ => 0
        };
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var handCards = Owner.GetHand();
        if (handCards.Count > 0) await CardCmd.Discard(ctx, handCards);
        var rarest = Owner.GetDraw()
            .OrderByDescending(c => RarityLevel(c.Rarity))
            .Take(3)
            .ToList();
        await CardPileCmd.Add(rarest, PileType.Hand);
    }
}
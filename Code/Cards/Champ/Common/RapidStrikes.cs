using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class RapidStrikes : ChampCardModel
{
    public RapidStrikes() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).WithHitCount(2).Execute(ctx);
        PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(c => c.Tags.Contains(CardTag.Strike))
            .ToList()
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection)
            .FirstOrDefault()?
            .EnergyCost
            .SetThisTurn(0);
    }
}
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Chomp : SlimeBossCardModel
{
    public Chomp() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = Owner.GetHand(e => e.Tags.Contains(SlimeBossTag.Tackle))
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (card == null) return;
        if (IsUpgraded)
            card.SetToFreeThisCombat();
        else
            card.SetToFreeThisTurn();
    }
}
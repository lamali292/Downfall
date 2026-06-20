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

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = Owner.RunState.Rng.CombatCardSelection
            .NextItem(Owner.GetHand(e => e.Tags.Contains(SlimeBossTag.Tackle)));
        if (card == null) return;
        if (IsUpgraded)
            card.SetToFreeThisCombat();
        else
            card.SetToFreeThisTurn();
    }
}
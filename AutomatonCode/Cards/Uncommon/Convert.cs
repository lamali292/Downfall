using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Convert : AutomatonCardModel
{
    public Convert() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 1);
        WithUpgradingCardTip<Fuel>();
    }

    protected override bool ShouldGlowGoldInternal => Owner.StashPile.Any(e => e.Type == CardType.Status);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = Owner.RunState.Rng.CombatCardSelection
            .NextItem(Owner.StashPile.Where(e => e.Type == CardType.Status));
        var fuel = card?.CardScope?.CreateCard<Fuel>(card.Owner);
        if (fuel == null || card == null) return;
        if (IsUpgraded) CardCmd.Upgrade(fuel);
        await CardCmd.Transform(card, fuel);
    }
}
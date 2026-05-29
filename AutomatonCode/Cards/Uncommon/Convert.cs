using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Convert : AutomatonCardModel
{
    public Convert() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 1);
        WithUpgradingCardTip<Fuel>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = Owner.GetDraw(e => e.Type == CardType.Status).TakeRandom(1, Owner.RunState.Rng.CombatCardSelection)
            .FirstOrDefault();
        var fuel = card?.CardScope?.CreateCard<Fuel>(card.Owner);
        if (fuel == null || card == null) return;
        if (IsUpgraded) fuel.UpgradeInternal();
        await CardCmd.Transform(card, fuel);
    }
}
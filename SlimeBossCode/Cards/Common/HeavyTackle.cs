using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class HeavyTackle : SlimeBossCardModel
{
    public HeavyTackle() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(15, 5);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone) return Task.CompletedTask;
        var amount = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.CardPlay.Card.Type == CardType.Status && e.CardPlay.Card.Owner == Owner && e.HappenedThisTurn(CombatState));
        EnergyCost.AddThisTurn(-amount);
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Status) EnergyCost.AddThisTurn(-1);
        return Task.CompletedTask;
    }
}

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class LeechLife : SlimeBossCardModel
{
    public LeechLife() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
        WithDamage(8, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var attack = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var unblocked = attack.Results.SelectMany(e => e).Sum(e => e.UnblockedDamage);
        await CreatureCmd.Heal(Owner.Creature, unblocked);
    }
}
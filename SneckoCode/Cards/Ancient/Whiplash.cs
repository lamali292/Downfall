using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Ancient;

[Pool(typeof(SneckoCardPool))]
public class Whiplash : SneckoCardModel
{
    public Whiplash() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithOverflow();
        WithDamage(10, 2);
        WithPower<VenomPower>(6, 2);
        WithTip<WeakPower>();
        WithTip<VulnerablePower>();
        WithVar("PowerVar", 2,  1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<VenomPower>(ctx, this, cardPlay);
        await OverflowCmd.Overflow(overflowing, cardPlay, async () =>
        {
            if (CombatState == null) return;
            var x = DynamicVars["PowerVar"].BaseValue;
            await PowerCmd.Apply<WeakPower>(ctx, cardPlay.Target, x, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(ctx, cardPlay.Target, x, Owner.Creature, this);
        });
    }
}
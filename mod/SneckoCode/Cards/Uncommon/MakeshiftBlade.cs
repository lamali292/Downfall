using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MakeshiftBlade : SneckoCardModel, IHasGift
{
    public MakeshiftBlade() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            IsDebuff = true
        });
        WithDamage(9, 4);
        WithCards(3);
        WithVar("Debuffs", 3);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (cardPlay.Target != null && HasEnoughDebuffs(cardPlay.Target)) await CommonActions.Draw(this, ctx);
    }

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(HasEnoughDebuffs) ?? false;

    private bool HasEnoughDebuffs(Creature creature)
    {
        return creature.Powers.Count(ShouldCountPower) >= DynamicVars["Debuffs"].IntValue;
    }

    private static bool ShouldCountPower(PowerModel power)
    {
        return power.TypeForCurrentAmount == PowerType.Debuff && power is not ITemporaryPower;
    }
}
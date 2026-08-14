using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Whack : SneckoCardModel
{
    public Whack() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithEnergy(2);
        WithTip(StaticHoverTip.Block);

    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var a = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var damage = a.Results.SelectMany(e => e).Sum(e => e.TotalDamage);
        if (EnergyCost.GetResolved() == DynamicVars.Energy.IntValue) return;
        await CreatureCmd.GainBlock(Owner.Creature, damage, BlockProps.card, cardPlay);
    }
}
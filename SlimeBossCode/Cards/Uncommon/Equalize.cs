using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Equalize : SlimeBossCardModel, IHasConsumeEffect
{
    private decimal? _damageDealt;

    public Equalize() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithTip(SlimeBossTip.Consume);
        WithTip(StaticHoverTip.Block);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        // TODO make better to give values to consume
        _damageDealt = result.Results.SelectMany(e => e).Sum(e => e.BlockedDamage + e.UnblockedDamage);
        await SlimeBossCmd.Consume(ctx, this, cardPlay);
        _damageDealt = null;
    }

    public async Task ConsumeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, Creature target)
    {
        if (!_damageDealt.HasValue) return;
        var damage = _damageDealt.Value;
        if (damage <= 0) return;
        // TODO scale with dex?
        await CreatureCmd.GainBlock(Owner.Creature, damage, BlockProps.cardUnpowered, cardPlay);
    }
}

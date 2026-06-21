using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Belittle : SneckoCardModel, IHasGift
{
    public Belittle() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon,
            IsDebuff = true
        });
        WithCalculatedDamage(0, 9, CalcDamage, ValueProp.Unblockable | ValueProp.Move | ValueProp.Unpowered, 0, 3);
    }

    public Gift? Gift { get; set; }

    private static decimal CalcDamage(CardModel card, Creature? creature)
    {
        return creature?.Powers.Count(e => e.Type == PowerType.Debuff) ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
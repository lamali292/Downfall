using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SerpentsNest : SneckoCardModel, IHasGift
{
    public SerpentsNest() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon,
            Type = CardType.Power
        });
        WithPower<SerpentsNestPower>(7, 3, false);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<SerpentsNestPower>(ctx, this);
    }
}
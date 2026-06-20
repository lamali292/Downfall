using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class SnakeEyes : SneckoCardModel, IHasGift
{
    public SnakeEyes() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            Type = CardType.Skill
        });
        WithPower<SnakeEyesPower>(1, 1);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<SnakeEyesPower>(ctx, this);
    }
}
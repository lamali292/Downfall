using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class QueenOfPentacles : SneckoCardModel, IHasGift
{
    public QueenOfPentacles() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithGift(new Gift
        {
            IsDebuff = true
        });
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithPower<QueenOfPentaclesPower>(4);
        WithTip(StaticHoverTip.Block);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay).ConfigureAwait(false);
        await CommonActions.ApplySelf<QueenOfPentaclesPower>(ctx, this);
    }
}
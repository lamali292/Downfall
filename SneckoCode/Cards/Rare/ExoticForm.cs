using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class ExoticForm : SneckoCardModel, IHasGift
{
    public ExoticForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Rare
        });
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        this.WithPower<ExoticFormPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Zhen>();

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<ExoticFormPower>(ctx, this);
    }
}
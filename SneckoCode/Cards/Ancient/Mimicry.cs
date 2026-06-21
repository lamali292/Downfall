using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Ancient;

[Pool(typeof(SneckoCardPool))]
public class Mimicry : SneckoCardModel
{
    public Mimicry() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        this.WithPower<MimicryPower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MimicryPower>(ctx, this);
    }
}
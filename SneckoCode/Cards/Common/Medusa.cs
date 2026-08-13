using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Medusa : SneckoCardModel
{
    public Medusa() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hitCount = DownfallCmd.IsDebuffed(cardPlay.Target) ? 2 : 1;
        await CommonActions.CardAttack(this, cardPlay, hitCount).Execute(ctx);
    }
}
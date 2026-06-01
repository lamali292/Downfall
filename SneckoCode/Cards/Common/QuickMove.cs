using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class QuickMove : SneckoCardModel, IHasOverflowEffect
{
    public QuickMove() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        WithBlock(7, 3);
        this.WithOverflow();
        WithPower<VulnerablePower>(1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}
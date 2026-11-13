using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class QuickMove : SneckoCardModel
{
    public QuickMove() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        WithBlock(7, 3);
        WithOverflow();
        WithPower<VulnerablePower>(1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardBlock(this, cardPlay);
        await OverflowCmd.Overflow(overflowing, cardPlay, () => CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay));
    }
}
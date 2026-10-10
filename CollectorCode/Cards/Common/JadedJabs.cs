using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class JadedJabs : CollectorCardModel
{
    public JadedJabs() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithDamage(13, 2);
        WithTip<LuckyWick>();
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        if (card is JadedJabs usesPyredCards)
        {
            return usesPyredCards.PyredCard?.EnergyCost.GetAmountToSpend() ?? 0;
        }
        return 0;
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private CardModel? PyredCard { get; set; }
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var pyred = await PyreCmd.Pyre(ctx, this);
        if (pyred == null) return;
        PyredCard = pyred;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (PyredCard != null && PyredCard.EnergyCost.GetAmountToSpend() > 0)
        {
            await DownfallCardCmd.GiveCards<LuckyWick>(Owner, PileType.Hand, PyredCard.EnergyCost.GetAmountToSpend());
        }
        PyredCard = null;
    }


}
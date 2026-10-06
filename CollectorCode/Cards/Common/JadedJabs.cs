using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class JadedJabs : CollectorCardModel
{
    public JadedJabs() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithCalculatedDamage(14, 3, Calc, DamageProps.card, 2, 1);
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
        PyredCard = null;
    }


}
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
public class JadedJabs : CollectorCardModel, IUsesPyredCards
{
    private CardModel? _pyredCard = null;
    
    public JadedJabs() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithCalculatedDamage(14, 3, Calc, DamageProps.card, 2, 1);
    }

    private decimal Calc(CardModel card, Creature? arg2)
    {
        var cost = _pyredCard?.EnergyCost.GetAmountToSpend() ?? 0;
        return (cost * (_currentUpgradeLevel >= 1 ? 4 : 3));
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public IEnumerable<CardModel> PyredCards { get; set; } = [];
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        _pyredCard = PyredCards.FirstOrDefault();
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }


}
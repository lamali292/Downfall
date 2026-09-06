using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class FlameLash : CollectorCardModel, IUsesPyredCards
{
    public FlameLash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithTip(CardKeyword.Exhaust);
        WithDamage(8, 4);
        WithEnergy(2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public IEnumerable<CardModel> PyredCards { get; set; } = [];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cost = PyredCards.FirstOrDefault()?.EnergyCost.GetAmountToSpend() ?? 0;
        if (cost >= DynamicVars.Energy.IntValue)
        {
            await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCardCompatibility(this, cardPlay)
                .TargetingAllOpponents(CombatState!).Execute(ctx);
        }
        else
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        }
        
       
    }
}
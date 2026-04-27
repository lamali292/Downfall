using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class FlameLash : CollectorCardModel
{
    public FlameLash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPyre();
        WithDamage(8, 4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (PyredCard == null || !PyredCard.DynamicVars.ContainsKey("Damage")) return;
        DynamicVars.Damage.UpgradeValueBy(PyredCard.DynamicVars.Damage.BaseValue);
    }
}
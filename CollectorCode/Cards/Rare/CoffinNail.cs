using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class CoffinNail : CollectorCardModel
{
    public CoffinNail() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithVar("Increase", 6, 2);
        WithPower<CopyNextTurnPower>(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;

        var power = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this);
        if (power == null) return;
        power.Card = this;
        power.OnAdd = c => c.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].BaseValue);
    }
}
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class InertBlade : SneckoCardModel
{
    public InertBlade() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithCards(3, 1);
        WithPower<StrengthPower>(3, 1);
        WithEnergy(1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cost = EnergyCost.GetResolved();
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (cost < 1) return;
        await CommonActions.Draw(this, ctx);
        if (cost < 2) return;
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        if (cost < 3) return;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
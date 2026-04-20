using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class MultiBeam : GuardianCardModel, ITickCard
{
    public MultiBeam() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(6, 2);
        WithVar("Increase", 2, 1);
    }


    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(x).Execute(ctx);
        
    }

    public Task OnTick(PlayerChoiceContext ctx)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        return Task.CompletedTask;
    }
}
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class MultiBeam : GuardianCardModel, ITickCard
{
    public MultiBeam() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(6, 2);
        WithVar("Increase", 2, 1);
    }

    protected override Artist Artist => Artist.Get<Magerblutooth>();


    protected override bool HasEnergyCostX => true;

    public Task OnTick(PlayerChoiceContext ctx)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(x).Execute(ctx);
    }
}
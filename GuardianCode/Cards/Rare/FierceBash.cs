using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class FierceBash : GuardianCardModel, ITickCard
{
    public FierceBash() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(18, 4);
        WithVar("Increase", 2);
        WithTip(GuardianTip.Stasis);
        WithTip(GuardianTip.Tick);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


    public Task OnTick(PlayerChoiceContext ctx)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        return Task.CompletedTask;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.PutIntoStasis(this, ctx, this);
    }
}
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class FierceBash : GuardianCardModel, ITickCard
{
    public FierceBash() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(18, 4);
        WithVar("Increase", 2);
        WithTip(DownfallTip.Stasis);
        WithTip(DownfallTip.Tick);
    }


    public Task OnTick(PlayerChoiceContext ctx)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.PutIntoStasis(this, ctx);
    }
}
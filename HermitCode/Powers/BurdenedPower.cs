using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Powers;

public class BurdenedPower : HermitPowerModel, IHasSecondAmount
{
    public BurdenedPower()
    {
        WithCards(0);
    }
    
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner.Player)
            return;
        Flash();
        await DownfallCardCmd.GiveCards<Decay>(player, PileType.Hand, DynamicVars.Cards.BaseValue);
        await PowerCmd.Apply<VigorPower>(ctx, Owner, Amount, Owner, null);
    }

    public void IncrementSelfDamage()
    {
        AssertMutable();
        ++DynamicVars.Cards.BaseValue;
        this.InvokeSecondAmountChanged();
    }

    public string GetSecondAmount() => $"{DynamicVars.Cards.IntValue}";
}
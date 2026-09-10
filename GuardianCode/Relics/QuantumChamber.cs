using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class QuantumChamber : GuardianRelicModel, IAfterCardEntersStasis
{

    public QuantumChamber() : base(RelicRarity.Rare)
    {
        WithTip(GuardianTip.Accelerate);
        WithTip(GuardianTip.Stasis);
    }
    
    private bool _usedThisTurn;

    public async Task AfterCardEntersStasis(PlayerChoiceContext ctx, CardModel card, AbstractModel source)
    {
        if (_usedThisTurn || card.Owner != Owner) return;
        Flash();
        await GuardianCmd.Accelerate(ctx, card, card.Owner);
        _usedThisTurn = true;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner) return Task.CompletedTask;
        _usedThisTurn = false;
        return Task.CompletedTask;
    }
}
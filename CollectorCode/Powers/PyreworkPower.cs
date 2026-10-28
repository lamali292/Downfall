using BaseLib.Patches.Localization;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class PyreworkPower : CollectorPowerModel, IAddDumbVariablesToPowerDescription, IAfterCardPyred
{

    public PyreworkPower()
    {
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithTorchheadDamage(5);//If this value changes, change the value in the main thing too.
        WithTip(CollectorKeyword.Torchhead);
    }

public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred.Owner.Creature != Owner || pyred.Type is not CardType.Status || Owner.Player == null) return;
        for (var v = 0; v < Amount; v++)
            await TorchheadCmd.Attack(ctx, this);
    }
    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        DynamicVars.TorchheadDamage.UpdatePowerPreview(this, CardPreviewMode.None, null, IsMutable);
        var shouldTargetAll = IsMutable && Owner.PetOwner != null && CollectorHook.ShouldTorchheadTargetAll(Owner.PetOwner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
    }

}
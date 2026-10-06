using BaseLib.Patches.Localization;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace Collector.CollectorCode.Powers;

public class PyreworkPower : CollectorPowerModel, IAddDumbVariablesToPowerDescription
{

    public PyreworkPower()
    {
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithTorchheadDamage(5);//If this value changes, change the value in the main thing too.
        WithTip(CollectorKeyword.Torchhead);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !(cardPlay.Card.Keywords.Contains(CollectorKeyword.Pyre) || cardPlay.Card.Keywords.Contains(CollectorKeyword.Megapyre))) return;

        for (var v = 0; v < Amount; v++)
        {
            if (Owner.Player != null && Owner.Player!.IsTorchheadAlive)
            {
                await TorchheadCmd.TorchheadAttack(this).ExecuteIfPresent(ctx);
            }
            else if (Owner.Player != null)
            {
                await TorchheadCmd.Kindle(ctx, Owner.Player!, 1, this);
            }
            else
            {
                break;
            }
        }

    }
  
    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        DynamicVars.TorchheadDamage.UpdatePowerPreview(this, CardPreviewMode.None, null, IsMutable);
        var shouldTargetAll = IsMutable && Owner.PetOwner != null && CollectorHook.ShouldTorchheadTargetAll(Owner.PetOwner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
    }

}
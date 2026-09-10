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
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !(cardPlay.Card.Keywords.Contains(CollectorKeyword.Pyre) || cardPlay.Card.Keywords.Contains(CollectorKeyword.Megapyre))) return;
        await CollectorCmd.TorchheadAttack(this).ExecuteIfPresent(ctx);
    }
  
    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        DynamicVars.TorchheadDamage.UpdatePowerPreview(this, CardPreviewMode.None, null, IsMutable);
        var shouldTargetAll = _owner?.PetOwner != null && CollectorHook.ShouldTorchheadTargetAll(_owner.PetOwner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
    }

}
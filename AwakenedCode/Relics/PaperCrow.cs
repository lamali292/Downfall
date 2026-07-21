using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class PaperCrow : AwakenedRelicModel, IModifyManaburnDamage
{
    public PaperCrow() : base(RelicRarity.Uncommon)
    {
        WithTip<ManaburnPower>();
    }
    
    public decimal ModifyManaburnDamage(decimal amount, decimal original, Player player)
    {
        if (Owner != player) return amount;
        return amount + original * 0.25m;
    }

    public Task AfterModifyingManaburnDamage(PlayerChoiceContext ctx, Player player)
    {
        Flash();
        return Task.CompletedTask;
    }
}
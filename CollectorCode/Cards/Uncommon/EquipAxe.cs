using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class EquipAxe : CollectorCardModel
{
    public EquipAxe() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKindle(6, 1);
        WithPower<EquipAxePower>(1, false);
        WithPower<StrengthPower>(2,1);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var torchhead = await CollectorCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<EquipAxePower>(ctx, this);
        await PowerCmd.Apply<StrengthPower>(ctx, torchhead, DynamicVars.Strength.BaseValue, Owner.Creature, this);
    }
}
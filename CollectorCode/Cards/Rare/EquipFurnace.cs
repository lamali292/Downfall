using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class EquipFurnace : CollectorCardModel
{
    public EquipFurnace() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<EquipFurnacePower>(1, false);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip<Ember>();
        WithTip<Burn>();
        WithTip<Soot>();
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<EquipFurnacePower>(ctx, this);
    }
}
using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Degrade : CollectorCardModel
{
    public Degrade() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Exhaust);
        
        WithPower<MiasmaPower>(2, 1);
        WithPower<StrengthPower>(1);
    }

    protected override bool HasEnergyCostX => true;
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var x = ResolveEnergyXValue();
        var strength = -DynamicVars.Strength.BaseValue * x;
        var miasma = DynamicVars.Power<MiasmaPower>().BaseValue * x;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(ctx, cardPlay.Target, strength, Owner.Creature, this);
        await PowerCmd.Apply<MiasmaPower>(ctx, cardPlay.Target, miasma, Owner.Creature, this);
        
        
    }
}
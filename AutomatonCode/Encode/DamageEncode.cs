using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class DamageEncode : Encodable
{
    public override TargetType Target => TargetType.AnyEnemy;
    public override CardType Type => CardType.Attack;
    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        if (target == null) return Task.CompletedTask;
        if (model is CardModel card)
        {
            return DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
                .FromCardCompatibility(card, cardPlay)
                .Targeting(target)
                .Execute(ctx);
        }
        return DownfallCreatureCmd.Damage(ctx, target, model.GetDynamicVars().Damage.BaseValue, ValueProp.Unpowered,
            model.GetCreature(), null, null);
    }
    public override DynamicVar FunctionDynamicVar => new DamageVar(0, ValueProp.Move);
    public override DynamicVar DynamicVar(AbstractModel model) => model.GetDynamicVars().Damage;
}
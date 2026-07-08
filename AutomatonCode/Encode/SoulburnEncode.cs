using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Extensions;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Encode;

public class SoulburnEncode : Encodable
{
    public override TargetType Target => TargetType.AllEnemies;
    public override CardType Type => CardType.Skill;
    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        var creature = model.GetCreature();
        var combatState = creature.CombatState;
        if (combatState == null) return Task.CompletedTask;
        return PowerCmd.Apply<SoulBurnPower>(ctx, combatState.HittableEnemies,
            model.GetDynamicVars().Power<SoulBurnPower>().BaseValue, creature, model as CardModel);
    }
    
    public override DynamicVar FunctionDynamicVar => new PowerVar<SoulBurnPower>(0);
    public override IEnumerable<IHoverTip> HoverTips(AbstractModel model) => [HoverTipFactory.FromPower<SoulBurnPower>()];
    public override DynamicVar DynamicVar(AbstractModel model) => model.GetDynamicVars().Power<SoulBurnPower>();
}
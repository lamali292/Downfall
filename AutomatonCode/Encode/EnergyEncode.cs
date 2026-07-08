using Automaton.AutomatonCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Encode;

public class EnergyEncode : Encodable
{
    public override TargetType Target => TargetType.Self;
    public override CardType Type => CardType.Skill;
    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        var player = model.GetCreature().Player;
        return player == null ?
            Task.CompletedTask : 
            PlayerCmd.GainEnergy(model.GetDynamicVars().Energy.IntValue, player);
    }
    public override DynamicVar FunctionDynamicVar => new EnergyVar(0);
    public override IEnumerable<IHoverTip> HoverTips(AbstractModel model) => [GetEnergyTip(model)];

    private IHoverTip GetEnergyTip(AbstractModel model)
    {
        return model switch
        {
            PowerModel power => HoverTipFactory.ForEnergy(power),
            RelicModel relic => HoverTipFactory.ForEnergy(relic),
            CardModel card => HoverTipFactory.ForEnergy(card),
            PotionModel potion => HoverTipFactory.ForEnergy(potion),
            _ => throw new Exception("Unknown model")
        };
    }
    
    public override DynamicVar DynamicVar(AbstractModel model) => model.GetDynamicVars().Energy;
}
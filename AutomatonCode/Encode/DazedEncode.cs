using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Encode;

public class DazedEncode : Encodable
{
    public override TargetType Target => TargetType.Self;
    public override CardType Type => CardType.Skill;
    public override DynamicVar FunctionDynamicVar => new("Dazed", 0);

    public override Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay)
    {
        var player = model.Creature.Player;
        return player == null
            ? Task.CompletedTask
            : DownfallCardCmd.GiveCards<Dazed>(player, PileType.Draw, model.DynamicVars["Dazed"].BaseValue, CardPilePosition.Random);
    }

    public override IEnumerable<IHoverTip> HoverTips(AbstractModel model)
    {
        return [HoverTipFactory.FromCard<Dazed>()];
    }

    public override DynamicVar DynamicVar(AbstractModel model)
    {
        return model.DynamicVars["Dazed"];
    }
}
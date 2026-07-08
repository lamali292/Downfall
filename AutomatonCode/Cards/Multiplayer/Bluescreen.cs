using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Multiplayer;

[Pool(typeof(AutomatonCardPool))]
public class Bluescreen : AutomatonCardModel
{
    public Bluescreen() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithBlock(12, 5);
        this.WithTip<Error>();
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Target?.Player;
        if (player == null) return;
        await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await DownfallCardCmd.GiveCard<Error>(player, PileType.Draw, CardPilePosition.Top);
    }
}
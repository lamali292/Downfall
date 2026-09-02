using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class FadeOut : HermitCardModel
{
    public FadeOut() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithBlock(9, 3);
        WithTip<Clumsy>();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var player in Owner.AllTeammates)
        {
            await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block.IntValue, DynamicVars.Block.Props, cardPlay);
            await DownfallCardCmd.GiveCard<Clumsy>(player, PileType.Discard, creator: Owner);
        }
    }
}
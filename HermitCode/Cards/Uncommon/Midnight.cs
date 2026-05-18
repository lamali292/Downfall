using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Curse;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Midnight : HermitCardModel
{
    public Midnight() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithBlock(12, 3);
        WithTip(typeof(ImpendingDoom));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await DownfallCardCmd.GiveCard<ImpendingDoom>(Owner, PileType.Hand);
    }
}